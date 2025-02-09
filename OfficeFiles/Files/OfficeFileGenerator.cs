namespace OfficeFileAccessor.OfficeFiles.Files;

public class OfficeFileGenerator(ILogger<OfficeFileGenerator> Logger): IOfficeFileGenerator
{
    public OfficeFile? Generate(string name, string fileName, string mimeType, List<Worksheets.Cell> cells)
    {

        Logger.LogWarning("Not implemented");
        return null;
    }
}
