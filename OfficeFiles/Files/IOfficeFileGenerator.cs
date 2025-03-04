namespace OfficeFileAccessor.OfficeFiles.Files;

public interface IOfficeFileGenerator
{
    List<OfficeFileTableGroup> Generate(Worksheets.PrintArea printArea,
        List<Worksheets.Cell> cells);
}
