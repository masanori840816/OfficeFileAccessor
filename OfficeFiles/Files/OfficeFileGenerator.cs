namespace OfficeFileAccessor.OfficeFiles.Files;

public class OfficeFileGenerator(ILogger<OfficeFileGenerator> Logger): IOfficeFileGenerator
{
    public List<OfficeFileTableGroup> Generate(string sheetName, Worksheets.PrintArea printArea,
        List<Worksheets.Cell> cells)
    {
        List<OfficeFileTableGroup> results = [];
        List<int> startColumns = [];
        int currentRow = 1;
        bool hasBorders = false;




        foreach(Worksheets.Cell cell in cells)
        {
            if(currentRow < cell.Address.Row)
            {
                currentRow = cell.Address.Row;
                Logger.LogInformation("Next row {r}", currentRow);
                hasBorders = false;
            }
            if(hasBorders)
            {
                if(cell.Borders.Right != Worksheets.BorderType.None && 
                    cell.Borders.Top != Worksheets.BorderType.None)
                {
                    Logger.LogInformation("End Group {c}", cell);
                    hasBorders = false;
                }
            }
            else 
            {
                if(cell.Borders.Left != Worksheets.BorderType.None && 
                    cell.Borders.Top != Worksheets.BorderType.None)
                {
                    hasBorders = true;
                    Logger.LogInformation("Start Group {c}", cell);
                    
                }
            }
            Logger.LogInformation("SheetCell {c}", cell);
        }
        Logger.LogWarning("Not implemented");
        return results;
    }
}
