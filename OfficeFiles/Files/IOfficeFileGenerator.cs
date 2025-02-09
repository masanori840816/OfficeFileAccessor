namespace OfficeFileAccessor.OfficeFiles.Files;

public interface IOfficeFileGenerator
{
    List<OfficeFileTableGroup> Generate(string sheetName, Worksheets.PrintArea printArea,
        List<Worksheets.Cell> cells);
}
