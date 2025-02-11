namespace OfficeFileAccessor.OfficeFiles.Files;

public class OfficeFileGenerator: IOfficeFileGenerator
{
    public List<OfficeFileTableGroup> Generate(string sheetName, Worksheets.PrintArea printArea,
        List<Worksheets.Cell> cells)
    {
        var ordered = cells.OrderBy(c => c.Address.Row)
            .ThenBy(c => c.Address.Column)
            .ToArray();
        List<OfficeFileTableGroup> results = [];        
        OfficeFileTableGroup lastGroup = new ()
        {
            DisplayOrder = 0,
            SheetName = sheetName,
        };
        results.Add(lastGroup);
        List<int> startColumns = [];
        
        int nextColumn = -1;
        for(int row = printArea.Start.Row; row <= printArea.End.Row; row++)
        {
            bool groupEnded = false;
            if(startColumns.Count <= 1)
            {
                startColumns = GetStartGroupColumns(cells, row, printArea);
                nextColumn = printArea.Start.Column;
                foreach(int c in startColumns)
                {
                    if(nextColumn < c)
                    {
                        nextColumn = c;
                        break;
                    }
                }
                if(nextColumn == printArea.Start.Column)
                {
                    nextColumn = printArea.End.Column;
                }
            }
            else if(CheckIsEndGroupRow(cells, row, printArea, startColumns))
            {
                groupEnded = true;
            }
            int currentRow = row;
            foreach(Worksheets.Cell cell in cells.Where(c => c.Address.Row == currentRow)
                .OrderBy(c => c.Address.Column))
            {
                if(startColumns.Count <= 1)
                {
                    lastGroup.Cells.Add(OfficeFileTableCell.Create(cell));
                    continue;
                }
                int column = cell.Address.Column;
                if(column >= nextColumn)
                {
                    OfficeFileTableGroup? nextGroup = results.FirstOrDefault(g => g.Cells.Any(c => 
                        c.CellAddress.Column == column));                    
                    if(nextGroup == null)
                    {
                        nextGroup = new ()
                        {
                            DisplayOrder = results.Count,
                            SheetName = sheetName,
                        };
                        results.Add(nextGroup);
                        lastGroup = nextGroup;
                    }
                    else
                    {
                        lastGroup = nextGroup;
                    }
                    int lastColumn = nextColumn;
                    foreach(int c in startColumns)
                    {
                        if(nextColumn < c)
                        {
                            nextColumn = c;
                            break;
                        }
                        if(lastColumn == nextColumn)
                        {
                            nextColumn = printArea.End.Column;
                        }                      
                    }
                    lastGroup.Cells.Add(OfficeFileTableCell.Create(cell));
                }
            }
            if(groupEnded)
            {
                groupEnded = false;
                startColumns.Clear();
                OfficeFileTableGroup nextGroup = new ()
                {
                    DisplayOrder = results.Count,
                    SheetName = sheetName,
                };
                results.Add(nextGroup);
                lastGroup = nextGroup;
            }
        }
        return results;
    }
    private static List<int> GetStartGroupColumns(List<Worksheets.Cell> cells, int row, Worksheets.PrintArea printArea)
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
                }
            }
            else 
            {
                if(cells[i].Borders.Left != Worksheets.BorderType.None &&
                    cells[i].Borders.Top != Worksheets.BorderType.None)
                {
                    hasBorders = true;
                    results.Add(cells[i].Address.Column);
                }
                else if(results.Count <= 0)
                {
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
