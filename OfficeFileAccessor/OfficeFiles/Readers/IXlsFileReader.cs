using OfficeFileAccessor.AppUsers.DTO;
using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles.Readers;

public interface IXlsFileReader
{
    OfficeFile? Read(IFormFile file, DisplayUser signinUser);
}