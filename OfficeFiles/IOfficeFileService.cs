
using OfficeFileAccessor.Apps;

namespace OfficeFileAccessor.OfficeFiles;

public interface IOfficeFileService
{
    Task<ApplicationResult> RegisterAsync(IFormFileCollection files);
}