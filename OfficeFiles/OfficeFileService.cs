
using OfficeFileAccessor.Apps;
using OfficeFileAccessor.OfficeFiles.Readers;

namespace OfficeFileAccessor.OfficeFiles;

public class OfficeFileService: IOfficeFileService
{
    private readonly ILogger<OfficeFileService> logger;
    private readonly XlsFileReader xlsFileReader;
    private readonly DocFileReader docFileReader;

    public OfficeFileService(ILogger<OfficeFileService> logger)
    {
        this.logger = logger;
        this.xlsFileReader = new XlsFileReader();
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
            var reader = GetReader(f);
            if(reader == null)
            {
                logger.LogWarning($"Invalid File Type: {f.ContentType}");
                continue;
            }
            reader.Read(f);
        }
        return ApplicationResult.GetFailedResult("Not implemented");
    }
    private IOfficeFileReader? GetReader(IFormFile file)
    {
        return file.ContentType switch
        {
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => xlsFileReader,
            "application/vnd.ms-excel.sheet.macroEnabled.12" => xlsFileReader,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => docFileReader,
            _ => null,
        };
    }
}