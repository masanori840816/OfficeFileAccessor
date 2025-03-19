using Microsoft.EntityFrameworkCore;
using OfficeFileAccessor.Apps;
using OfficeFileAccessor.OfficeFiles.DTO;
using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles.Repositories;

public class OfficeFiles(ILogger<OfficeFile> Logger, OfficeFileAccessorContext Context): IOfficeFiles
{
    public async Task<ApplicationResult> CreateAsync(OfficeFile newItem)
    {
        using var transaction = await Context.Database.BeginTransactionAsync();
        try
        {
            await Context.OfficeFiles.AddAsync( newItem );
            await Context.SaveChangesAsync();
            await transaction.CommitAsync();
            return ApplicationResult.GetSucceededResult();
        }
        catch(Exception ex)
        {
            Logger.LogError("Create officefile error {ex}", ex.Message);
            await transaction.RollbackAsync();
            return ApplicationResult.GetFailedResult("Failed creating OfficeFile");
        }
    }
    public async Task<List<PreviewOfficeFileSheets>> GetPreviewSheetsAsync(long fileId)
    {
        string sql = """
            SELECT ofile.id AS "FileId",
            ofile.file_name AS "FileName",
            sheet.id AS "SheetId",
            sheet.name AS "SheetName",
            sheet.display_order AS "DisplayOrder",
            usr.user_name AS "RegisterUser",
            ofile.last_update_date AS "LastUpdateDate"
            FROM office_file ofile
            INNER JOIN link_file_sheet lfs ON ofile.id = lfs.file_id
            INNER JOIN office_file_sheet sheet ON sheet.id = lfs.sheet_id
            INNER JOIN application_user usr ON ofile.register_user_id = usr.id
        """;
        return await Context.PreviewSheets.FromSqlRaw(sql)
            .Where(s => s.FileId == fileId)
            .ToListAsync();
    }
    public async Task<List<DisplayOfficeFileCell>> GetDisplayCellsAsync(long sheetId)
    {
        
        string sql = """
            SELECT sheet.id AS "SheetId",
                tgroup.id AS "GroupId",
                cell.id AS "CellId",
                tgroup.display_order AS "GroupDisplayOrder",
                tgroup.title AS "Title",
                cell.column AS "Column",
                cell.row AS "Row",
                cell.vertical_length AS "VerticalLength",
                cell.horizontal_length AS "HorizontalLength",
                cell.value AS "Value",
                cell.formula AS "Formula",
                cell.value_type AS "ValueType",
                cell.background_color AS "BackgroundColor",
                cell.editabled AS "Editabled",
                cell.vertical_writing AS "VerticalWriting",
                cell.text_rotation AS "TextRotation",
                border.left AS "BorderLeft",
                border.top AS "BorderTop",
                border.right AS "BorderRight",
                border.bottom AS "BorderBottom",
                font.font_name AS "FontName",
                font.font_size AS "FontSize",
                font.font_color AS "FontColor",
                font.bold AS "Bold",
                mtc.start_column AS "MergedStartColumn",
                mtc.start_row AS "MergedStartRow",
                mtc.end_column AS "MergedEndColumn",
                mtc.end_row AS "MergedEndRow"
                FROM office_file_sheet sheet
                INNER JOIN link_sheet_group lsg ON sheet.id = lsg.sheet_id
                INNER JOIN table_group tgroup ON tgroup.id = lsg.table_group_id
                INNER JOIN link_group_cell lgc ON tgroup.id = lgc.table_group_id
                INNER JOIN table_cell cell ON cell.id = lgc.table_cell_id
                LEFT JOIN table_cell_borders border ON cell.id = border.table_cell_id
                LEFT JOIN table_cell_font_format font ON cell.id = font.table_cell_id
                LEFT JOIN merged_table_cell mtc ON cell.id = mtc.table_cell_id
            """;
        return await Context.DisplayCells.FromSqlRaw(sql)
            .Where(c => c.SheetId == sheetId)
            .OrderBy(c => c.GroupDisplayOrder)
            .ThenBy(c => c.Row)
            .ThenBy(c => c.Column)
            .ToListAsync();
    }
    public async Task<List<TableColumnWidth>> GetColumnWidthsAsync(long sheetId)
    {
        string sql = """
            SELECT width.id, width.column, width.width FROM table_column_width width
            INNER JOIN link_sheet_column_width lsw ON width.id = lsw.column_width_id
            WHERE lsw.sheet_id = 
            """;
        sql += sheetId;
        return await Context.ColumnWidths.FromSqlRaw(sql)
            .ToListAsync();
    }
    public async Task<List<TableRowHeight>> GetRowHeightsAsync(long sheetId)
    {
        string sql = """
            SELECT height.id, height.row, height.height FROM table_row_height height
            INNER JOIN link_sheet_row_height lrh ON height.id = lrh.row_height_id
            WHERE lrh.sheet_id = 
            """;
        sql += sheetId;
        return await Context.RowHeights.FromSqlRaw(sql)
            .ToListAsync();
    }
    public async Task<List<SearchOfficeFile>> SearchOfficeFilesAsync(string? fileName, string? userName, 
        string? updateDateFrom, string? updateDateTo, int? limit)
    {
        string sql = """
            SELECT ofile.id AS "FileId",
                    ofile.file_name AS "FileName",
                    ofile.register_user_id AS "RegisterUserId",
                    auser.user_name AS "UserName",
                    ofile.version AS "Version",
                    1 AS "UseCount",
                    ofile.last_update_date AS "LastUpdateDate"
                FROM office_file ofile
                INNER JOIN application_user auser ON ofile.register_user_id = auser.id
            """;
        IQueryable<SearchOfficeFile> query = Context.SearchOfficeFiles.FromSqlRaw(sql);
        if(string.IsNullOrEmpty(fileName) == false)
        {
            query = query.Where(f => f.FileName.Contains(fileName));
        }
        if(string.IsNullOrEmpty(userName) == false)
        {
            query = query.Where(f => f.UserName.Contains(userName));
        }
        if(string.IsNullOrEmpty(updateDateFrom) == false &&
            DateTime.TryParse($"{updateDateFrom} 00:00:00", out var dateFrom))
        {
            DateTime uDateFrom = dateFrom.ToUniversalTime();
            query = query.Where(p => p.LastUpdateDate >= uDateFrom);
        }
        if(string.IsNullOrEmpty(updateDateTo) == false &&
            DateTime.TryParse($"{updateDateTo} 23:59:59", out var dateTo))
        {
            DateTime uDateTo = dateTo.ToUniversalTime();
            query = query.Where(p => p.LastUpdateDate <= uDateTo);
        }
        int takeCount = 10;
        if(limit != null && limit > 0)
        {
            takeCount = limit.Value;
        }
        return await Task.Run(() => {
            return query.AsEnumerable()
                .GroupBy(p => p.FileName)
                .Select(p => p.OrderByDescending(w => w.Version).First())
                .OrderByDescending(p => p.LastUpdateDate)
                .Take(takeCount)
                .ToList();
        });
    }
}