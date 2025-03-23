
using OfficeFileAccessor.Files;
using OfficeFileAccessor.Files.DTO;
using OfficeFileAccessor.OfficeFiles.DTO;
using OfficeFileAccessor.OfficeFiles.Entities;
using OfficeFileAccessor.OfficeFiles.Records.Entities;
using OfficeFileAccessor.OfficeFiles.Records.Repositories;
using OfficeFileAccessor.OfficeFiles.Repositories;

namespace OfficeFileAccessor.OfficeFiles;

public class WorkRecordService(ILogger<WorkRecordService> Logger, IJsonCamelCaseOption JsonOption, 
    IOfficeFiles OfficeFiles, IWorkRecords WorkRecords): IWorkRecordService
{
    public async Task<DownloadFile> DonwloadFileAsync(long recordId)
    {
        WorkRecord? workRecord = await WorkRecords.GetWorkRecordAsync(recordId);
        if(workRecord == null)
        {
            return RegisterFileResult.GenerateFailedResult("Record not found", JsonOption.Get());
        }
        OfficeFile? file = await OfficeFiles.GetFileAsync(workRecord.OfficeFileId);
        if(file?.OfficeFileData == null)
        {
            return RegisterFileResult.GenerateFailedResult("File not found", JsonOption.Get());
        }
        List<EditabledCell> editabledCells = await OfficeFiles.GetEditabledCellsByFileIdAsync(workRecord.OfficeFileId);
        foreach(var c in editabledCells)
        {
            Logger.LogWarning("Cell fid{fid} sheet:{s} cel:{c}", c.FileId, c.SheetName, c.CellId);
        }
        return RegisterFileResult.GenerateFailedResult("not implemented", JsonOption.Get());
    }
}