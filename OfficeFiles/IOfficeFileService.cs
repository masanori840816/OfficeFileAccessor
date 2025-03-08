
using OfficeFileAccessor.Apps;
using OfficeFileAccessor.AppUsers.DTO;
using OfficeFileAccessor.Files.DTO;
using OfficeFileAccessor.OfficeFiles.DTO;
using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles;

public interface IOfficeFileService
{
    Task<DownloadFile> RegisterAsync(IFormFileCollection files, DisplayUser signinUser);
    /// <summary>
    /// Get an office file and sheets by id.
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    Task<List<PreviewOfficeFileSheets>> GetPreviewSheetsAsync(long fileId);
    /// <summary>
    /// Get a office file sheet by id.
    /// </summary>
    /// <param name="sheetId"></param>
    /// <returns></returns>
    Task<OfficeFile?> GetOfficeFileSheetAsync(long? sheetId);
}