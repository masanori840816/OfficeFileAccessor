using OfficeFileAccessor.Apps;
using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles.Repositories;

public interface IOfficeFile
{
    Task<ApplicationResult> CreateAsync(OfficeFile newItem);
}