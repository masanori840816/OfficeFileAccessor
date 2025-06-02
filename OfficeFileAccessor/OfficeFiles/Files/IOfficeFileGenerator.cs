using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles.Files;

public interface IOfficeFileGenerator
{
    List<TableGroup> Generate(Worksheets.PrintArea printArea,
        List<Worksheets.Cell> cells);
}
