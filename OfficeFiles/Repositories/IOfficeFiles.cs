using OfficeFileAccessor.Apps;
using OfficeFileAccessor.OfficeFiles.DTO;
using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles.Repositories;

public interface IOfficeFiles
{
    Task<ApplicationResult> CreateAsync(OfficeFile newItem);
    Task<List<PreviewOfficeFileSheets>> GetPreviewSheetsAsync(long fileId);
    Task<OfficeFile?> GetOfficeFileSheetAsync(long? sheetId);
}