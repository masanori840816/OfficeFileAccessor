using OfficeFileAccessor.OfficeFiles.DTO;
using OfficeFileAccessor.OfficeFiles.Entities;
using OfficeFileAccessor.OfficeFiles.Records.Entities;

namespace OfficeFileAccessor.OfficeFiles.Writers;

public interface IXlsFileWriter
{
    byte[]? WriteRecords(OfficeFile file, WorkRecord record, List<EditabledCell> editabledCells);
}