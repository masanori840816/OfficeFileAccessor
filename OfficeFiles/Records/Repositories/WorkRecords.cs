using Microsoft.EntityFrameworkCore;
using OfficeFileAccessor.OfficeFiles.Records.Entities;

namespace OfficeFileAccessor.OfficeFiles.Records.Repositories;

public class WorkRecords(ILogger<WorkRecords> Logger, OfficeFileAccessorContext Context): IWorkRecords
{
    public async Task<WorkRecord?> GetWorkRecordAsync(long recordId)
    {
        WorkRecord? result = await Context.WorkRecords.FirstOrDefaultAsync(r => r.Id == recordId);
        if(result == null)
        {
            return null;
        }
        result.InputRecords.AddRange(await Context.InputRecords.Where(r => r.WorkRecordId == recordId)
            .GroupBy(p => p.TableCellId)
            .Select(p => p.OrderByDescending(w => w.Revision).First())
            .ToListAsync());
        return result;

    }
}