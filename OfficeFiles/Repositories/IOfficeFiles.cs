using OfficeFileAccessor.Apps;
using OfficeFileAccessor.OfficeFiles.DTO;
using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles.Repositories;

public interface IOfficeFiles
{
    Task<ApplicationResult> CreateAsync(OfficeFile newItem);
    Task<List<PreviewOfficeFileSheets>> GetPreviewSheetsAsync(long fileId);
    Task<List<DisplayOfficeFileCell>> GetDisplayCellsAsync(long sheetId);
    Task<List<TableColumnWidth>> GetColumnWidthsAsync(long sheetId);
    Task<List<TableRowHeight>> GetRowHeightsAsync(long sheetId);
    Task<List<SearchOfficeFile>> SearchOfficeFilesAsync(string? fileName, string? userName, 
        string? updateDateFrom, string? updateDateTo, int? limit);
    Task<OfficeFile?> GetFileAsync(long fileId);
}