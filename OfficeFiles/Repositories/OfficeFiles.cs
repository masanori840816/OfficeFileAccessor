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
            usr.user_name AS "RegisterUser"
            FROM office_file ofile
            INNER JOIN link_file_sheet lfs ON ofile.id = lfs.file_id
            INNER JOIN office_file_sheet sheet ON sheet.id = lfs.sheet_id
            INNER JOIN application_user usr ON ofile.register_user_id = usr.id
        """;
        return await Context.PreviewSheets.FromSqlRaw(sql)
            .Where(s => s.FileId == fileId)
            .ToListAsync();
    }
}