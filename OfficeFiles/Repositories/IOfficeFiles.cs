using OfficeFileAccessor.Apps;
using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles.Repositories;

public interface IOfficeFiles
{
    Task<ApplicationResult> CreateAsync(OfficeFile newItem);
}