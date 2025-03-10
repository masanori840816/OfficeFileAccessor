
using OfficeFileAccessor.Apps;
using OfficeFileAccessor.AppUsers.DTO;
using OfficeFileAccessor.Files;
using OfficeFileAccessor.Files.DTO;
using OfficeFileAccessor.OfficeFiles.DTO;
using OfficeFileAccessor.OfficeFiles.Entities;
using OfficeFileAccessor.OfficeFiles.Readers;
using OfficeFileAccessor.OfficeFiles.Repositories;

namespace OfficeFileAccessor.OfficeFiles;

public class OfficeFileService(ILogger<OfficeFileService> Logger, IXlsFileReader XlsFileReader,
        IJsonCamelCaseOption JsonOption, IOfficeFiles OfficeFiles): IOfficeFileService
{
    private readonly DocFileReader docFileReader = new ();
    
    public async Task<DownloadFile> RegisterAsync(IFormFileCollection files, DisplayUser signinUser)
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
                    file = XlsFileReader.Read(f, signinUser);
                    if(file == null)
                    {
                        Logger.LogWarning("Faile reading the file");
                    } else {
                        ApplicationResult createResult = await OfficeFiles.CreateAsync(file);
                        Logger.LogWarning("CREATE Result {r}", createResult);
                    }
                    
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
    /// <summary>
    /// Get an office file and sheets by id.
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    public async Task<List<PreviewOfficeFileSheets>> GetPreviewSheetsAsync(long fileId)
    {
        return await OfficeFiles.GetPreviewSheetsAsync(fileId);
    }
    /// <summary>
    /// Get a office file sheet by ids.
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="sheetId"></param>
    /// <returns></returns>
    public async Task<DisplayOfficeFileSheet?> GetOfficeFileSheetAsync(long sheetId)
    {
        List<DisplayOfficeFileCell> cells = await OfficeFiles.GetDisplayCellsAsync(sheetId);
        if(cells.Count <= 0)
        {
            return null;
        }
        DisplayOfficeFileSheet result = new (
            SheetId: sheetId,
            ColumnWidths: await OfficeFiles.GetColumnWidthsAsync(sheetId),
            RowHeights: await OfficeFiles.GetRowHeightsAsync(sheetId),
            Cells: cells
        );
        return result;
    }
}