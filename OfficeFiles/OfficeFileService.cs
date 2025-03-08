
using OfficeFileAccessor.Apps;
using OfficeFileAccessor.Files;
using OfficeFileAccessor.Files.DTO;
using OfficeFileAccessor.OfficeFiles.DTO;
using OfficeFileAccessor.OfficeFiles.Entities;
using OfficeFileAccessor.OfficeFiles.Readers;

namespace OfficeFileAccessor.OfficeFiles;

public class OfficeFileService: IOfficeFileService
{
    private readonly ILogger<OfficeFileService> Logger;
    private readonly IXlsFileReader XlsFileReader;
    private readonly DocFileReader docFileReader;
    private readonly IJsonCamelCaseOption JsonOption;

    public OfficeFileService(ILogger<OfficeFileService> Logger, IXlsFileReader XlsFileReader,
        IJsonCamelCaseOption JsonOption)
    {
        this.Logger = Logger;
        this.XlsFileReader = XlsFileReader;
        this.docFileReader = new DocFileReader();
        this.JsonOption = JsonOption;
    }
    public async Task<DownloadFile> RegisterAsync(IFormFileCollection files)
    {
        OfficeFile? file = null;
        foreach(var f in files!)
        {
            if(f == null)
            {
                Logger.LogWarning("File was null");
                continue;
            }
            switch(f.ContentType)
            {
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                case "application/vnd.ms-excel.sheet.macroEnabled.12":
                    file = XlsFileReader.Read(f);
                    break;
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    docFileReader.Read(f);
                    break;
                default:
                    Logger.LogWarning($"Invalid File Type: {f.ContentType}");
                    continue;
            }
            break;
        }
        if(file == null)
        {
            return RegisterFileResult.GenerateFailedResult("Failed loading file", JsonOption.Get());
        }
        RegisterFileResult result = new ()
        {
            Result = ApplicationResult.GetSucceededResult(),
            File = file,
        };

        return result.GenerateDownloadFile(JsonOption.Get());
    }
}