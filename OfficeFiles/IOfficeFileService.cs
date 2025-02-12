
using OfficeFileAccessor.Apps;
using OfficeFileAccessor.Files.DTO;

namespace OfficeFileAccessor.OfficeFiles;

public interface IOfficeFileService
{
    Task<DownloadFile> RegisterAsync(IFormFileCollection files);
}