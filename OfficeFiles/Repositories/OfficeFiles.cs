using OfficeFileAccessor.Apps;
using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles.Repositories;

public class OfficeFiles(ILogger<OfficeFile> Logger, OfficeFileAccessorContext Context): IOfficeFiles
{
    public async Task<ApplicationResult> CreateAsync(OfficeFile newItem)
    {
        using var transaction = await Context.Database.BeginTransactionAsync();
        try
        {
            await Context.OfficeFiles.AddAsync( newItem );
            await Context.SaveChangesAsync();
            await transaction.CommitAsync();
            return ApplicationResult.GetSucceededResult();
        }
        catch(Exception ex)
        {
            Logger.LogError("Create officefile error {ex}", ex.Message);
            await transaction.RollbackAsync();
            return ApplicationResult.GetFailedResult("Failed creating OfficeFile");
        }
    }
}