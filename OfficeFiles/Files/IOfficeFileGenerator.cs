namespace OfficeFileAccessor.OfficeFiles.Files;

public interface IOfficeFileGenerator
{
    OfficeFile? Generate(string name, string fileName, string mimeType, List<Worksheets.Cell> cells);
}
