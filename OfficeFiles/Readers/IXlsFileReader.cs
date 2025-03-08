using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles.Readers;

public interface IXlsFileReader
{
    OfficeFile? Read(IFormFile file);
}