
using OfficeFileAccessor.Files.DTO;

namespace OfficeFileAccessor.OfficeFiles;

public interface IWorkRecordService
{
    Task<DownloadFile> DonwloadFileAsync(long recordId);
}