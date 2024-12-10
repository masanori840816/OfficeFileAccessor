
using OfficeFileAccessor.Apps;

namespace OfficeFileAccessor.OfficeFiles;

public class OfficeFileService(ILogger<OfficeFileService> logger): IOfficeFileService
{
    public async Task<ApplicationResult> RegisterAsync(IFormFileCollection files)
    {
        foreach(var f in files!)
        {
            logger.LogInformation($"FileName: {f?.FileName} Type: {f?.ContentType} Desc:{f?.ContentDisposition}");
        }
        return ApplicationResult.GetFailedResult("Not implemented");
    }
}