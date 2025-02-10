namespace OfficeFileAccessor.OfficeFiles.Files;

public class OfficeFileGenerator(ILogger<OfficeFileGenerator> Logger): IOfficeFileGenerator
{
    public List<OfficeFileTableGroup> Generate(string sheetName, Worksheets.PrintArea printArea,
        List<Worksheets.Cell> cells)
    {
        var ordered = cells.OrderBy(c => c.Address.Row)
            .ThenBy(c => c.Address.Column)
            .ToArray();
        List<OfficeFileTableGroup> results = [];
        results.Add(new OfficeFileTableGroup
        {
            DisplayOrder = 0,
            SheetName = sheetName,
        });
        List<int> startColumns = [];
        for(int row = printArea.Start.Row; row <= printArea.End.Row; row++)
        {
            if(startColumns.Count <= 1)
            {
                startColumns = GetStartGroupColumns(cells, row, printArea);
            }
            else if(CheckIsEndGroupRow(cells, row, printArea, startColumns))
            {
                Logger.LogInformation("Next Gropp {row}", row);
                startColumns.Clear();
            }
        }
        Logger.LogWarning("Not implemented");
        return results;
    }
    private List<int> GetStartGroupColumns(List<Worksheets.Cell> cells, int row, Worksheets.PrintArea printArea)
    {
        List<int> results = [];
        bool hasBorders = false;
        for(int i = 0; i < cells.Count; i++)
        {
            if(row > cells[i].Address.Row)
            {
                continue;
            }
            if(row < cells[i].Address.Row)
            {
                break;
            }
            if(hasBorders)
            {
                if(cells[i].Borders.Right != Worksheets.BorderType.None &&
                    cells[i].Borders.Top != Worksheets.BorderType.None &&
                    (cells[i].Address.Column >= printArea.End.Column || cells[i + 1].Borders.Top == Worksheets.BorderType.None))
                {
                    hasBorders = false;
                    Logger.LogInformation("Group End column: {co} row: {r}", cells[i].Address.Column, row);
                }
            }
            else 
            {
                if(cells[i].Borders.Left != Worksheets.BorderType.None &&
                    cells[i].Borders.Top != Worksheets.BorderType.None)
                {
                    hasBorders = true;
                    results.Add(cells[i].Address.Column);
                    Logger.LogInformation("Group Start column: {co} row: {r}", cells[i].Address.Column, row);
                }
                else if(results.Count <= 0)
                {
                    Logger.LogInformation("Group Start empty column: {co} row: {r}", cells[i].Address.Column, row);
                    results.Add(cells[i].Address.Column);
                }
            }
        }
        return results;
    }
    private static bool CheckIsEndGroupRow(List<Worksheets.Cell> cells, int row, Worksheets.PrintArea printArea,
        List<int> startGroupColumns)
    {
        if(printArea.End.Row <= row)
        {
            return true;
        }
        for(int i = 0; i < cells.Count; i++)
        {
            if(row > cells[i].Address.Row)
            {
                continue;
            }
            if(row < cells[i].Address.Row)
            {
                break;
            }
            int column = cells[i].Address.Column;
            if(startGroupColumns.Any(s => s == column) == false)
            {
                continue;
            }
            if(cells[i].Borders.Left == Worksheets.BorderType.None ||
                    cells[i].Borders.Bottom == Worksheets.BorderType.None)
            {
                continue;
            }
            int nextRow = row + 1;
            if(cells.Any(c => c.Address.Row == nextRow &&
                (c.Borders.Left != Worksheets.BorderType.None ||
                c.Borders.Right != Worksheets.BorderType.None ||
                c.Borders.Bottom != Worksheets.BorderType.None)) == false)
            {
                return true;
            }
        }

        return false;
    }
}
