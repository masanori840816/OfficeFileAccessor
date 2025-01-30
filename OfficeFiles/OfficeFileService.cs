
using OfficeFileAccessor.Apps;
using OfficeFileAccessor.OfficeFiles.Readers;

namespace OfficeFileAccessor.OfficeFiles;

public class OfficeFileService: IOfficeFileService
{
    private readonly ILogger<OfficeFileService> logger;
    private readonly IXlsFileReader xlsFileReader;
    private readonly DocFileReader docFileReader;

    public OfficeFileService(ILogger<OfficeFileService> logger, IXlsFileReader xlsFileReader)
    {
        this.logger = logger;
        this.xlsFileReader = xlsFileReader;
        this.docFileReader = new DocFileReader();

    }
    public async Task<ApplicationResult> RegisterAsync(IFormFileCollection files)
    {
        foreach(var f in files!)
        {
            logger.LogInformation($"FileName: {f?.FileName} Type: {f?.ContentType} Desc:{f?.ContentDisposition}");
            if(f == null)
            {
                logger.LogWarning("File was null");
                continue;
            }
            switch(f.ContentType)
            {
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                case "application/vnd.ms-excel.sheet.macroEnabled.12":
                    xlsFileReader.Read(f);
                    break;
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    docFileReader.Read(f);
                    break;
                default:
                    logger.LogWarning($"Invalid File Type: {f.ContentType}");
                    continue;
            }
        }
        return ApplicationResult.GetFailedResult("Not implemented");
    }
}