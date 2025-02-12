using OfficeFileAccessor.OfficeFiles.Files;

namespace OfficeFileAccessor.OfficeFiles.Readers;

public interface IXlsFileReader
{
    OfficeFile? Read(IFormFile file);
}