
using OfficeFileAccessor.Files;
using OfficeFileAccessor.Files.DTO;
using OfficeFileAccessor.OfficeFiles.DTO;
using OfficeFileAccessor.OfficeFiles.Entities;
using OfficeFileAccessor.OfficeFiles.Records.Entities;
using OfficeFileAccessor.OfficeFiles.Records.Repositories;
using OfficeFileAccessor.OfficeFiles.Repositories;
using OfficeFileAccessor.OfficeFiles.Writers;

namespace OfficeFileAccessor.OfficeFiles;

public class WorkRecordService(ILogger<WorkRecordService> Logger, IJsonCamelCaseOption JsonOption, 
    IOfficeFiles OfficeFiles, IWorkRecords WorkRecords, IXlsFileWriter XlsFileWriter): IWorkRecordService
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
        byte[]? fileData = XlsFileWriter.WriteRecords(file, workRecord, editabledCells);
        if(fileData == null)
        {
            return RegisterFileResult.GenerateFailedResult("Failed writing records", JsonOption.Get());
        }
        return new DownloadFile(
            FileName: file.FileName,
            MimeType: file.MimeType,
            FileData: fileData
        );
    }
}