using OfficeFileAccessor.OfficeFiles.Records.Entities;

namespace OfficeFileAccessor.OfficeFiles.Records.Repositories;

public interface IWorkRecords
{
    Task<WorkRecord?> GetWorkRecordAsync(long recordId);
}