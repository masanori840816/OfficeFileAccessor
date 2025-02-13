namespace OfficeFileAccessor.OfficeFiles.Files;

public class OfficeFileGenerator(ILogger<OfficeFileGenerator> Logger): IOfficeFileGenerator
{
    private record GroupedCells
    {
        public required int StartColumn { get; init; }
        public required int DisplayOrder { get; init; }
        public List<Worksheets.Cell> Cells { get; init; } = [];
    }
    public List<OfficeFileTableGroup> Generate(string sheetName, Worksheets.PrintArea printArea,
        List<Worksheets.Cell> cells)
    {
        List<GroupedCells> groupedCells = GroupCells(printArea, cells);
        
        // TODO: for all groups
        GroupedCells firstGroup = groupedCells.First(g => g.Cells.Any(c => c.Borders.Left != Worksheets.BorderType.None));
        List<Worksheets.CellAddress> addedAddresses = [];
        List<OfficeFileTableCell> tableCells = [];
        foreach(Worksheets.Cell cell in firstGroup.Cells)
        {
            if(addedAddresses.Any(a => cell.Address == a))
            {
                continue;
            }

            if(cell.Borders.Left == Worksheets.BorderType.None ||
                cell.Borders.Top == Worksheets.BorderType.None)
            {
                continue;
            }
            Logger.LogWarning("-----------------");
            List<Worksheets.Cell> c = [cell];
            AddMergedCells(cell, c, firstGroup.Cells);
            c = GetRightTop(cell.Address, c, firstGroup.Cells);
            c = GetLeftBottom(cell.Address, c, firstGroup.Cells);
            foreach(var cel in c)
            {
                Logger.LogWarning("LCEll c: {c} right: {cel}", cell.Address, cel.Address);
            }
        }
        

        List<OfficeFileTableGroup> results = [];
        return results;
    }
    private static void AddMergedCells(Worksheets.Cell cell, List<Worksheets.Cell> current, List<Worksheets.Cell> allCells)
    {
        if(cell.Merged == false || cell.MergedCell == null)
        {
            return;
        }
        foreach(Worksheets.Cell c in allCells.Where(c => c.Address.Column >= cell.MergedCell.Start.Column && c.Address.Column <= cell.MergedCell.End.Column &&
                c.Address.Row >= cell.MergedCell.Start.Row && c.Address.Row <= cell.MergedCell.End.Row))
        {
            Worksheets.CellAddress address = c.Address;
            if(current.Any(cu => cu.Address == address) == false)
            {
                current.Add(c);
            }
        }        
    }
    private static List<Worksheets.Cell> GetRightTop(Worksheets.CellAddress baseAddress,
        List<Worksheets.Cell> current, List<Worksheets.Cell> allCells)
    {
        if(current.Any(ce => ce.Borders.Right != Worksheets.BorderType.None &&
                ce.Borders.Top != Worksheets.BorderType.None))
        {
            return current;
        }
        Worksheets.CellAddress rightAddress = Worksheets.CellAddress.Move(baseAddress, 1, 0);
        Worksheets.Cell? right = allCells.FirstOrDefault(c => c.Address == rightAddress);
        if(right == null)
        {
            return current;
        }
        current.Add(right);
        AddMergedCells(right, current, allCells);
        return GetRightTop(rightAddress, current, allCells);
    }
    private static List<Worksheets.Cell> GetLeftBottom(Worksheets.CellAddress baseAddress,
        List<Worksheets.Cell> current, List<Worksheets.Cell> allCells)
    {
        if(current.Any(ce => ce.Borders.Left != Worksheets.BorderType.None &&
                ce.Borders.Bottom != Worksheets.BorderType.None))
        {
            return current;
        }
        Worksheets.CellAddress bottomAddress = Worksheets.CellAddress.Move(baseAddress, 0, 1);
        Worksheets.Cell? bottom = allCells.FirstOrDefault(c => c.Address == bottomAddress);
        if(bottom == null)
        {
            return current;
        }
        current.Add(bottom);
        AddMergedCells(bottom, current, allCells);
        return GetLeftBottom(bottomAddress, current, allCells);
    }
    private static List<GroupedCells> GroupCells(Worksheets.PrintArea printArea, List<Worksheets.Cell> cells)
    {
        Worksheets.Cell[] ordered = [.. cells.OrderBy(c => c.Address.Row).ThenBy(c => c.Address.Column)];
        List<GroupedCells> results = [];
        GroupedCells lastGroup = new ()
        {
            StartColumn = printArea.Start.Column,
            DisplayOrder = 0,
        };
        results.Add(lastGroup);
        List<int> startColumns = [];
        for(int row = printArea.Start.Row; row <= printArea.End.Row; row++)
        {
            bool groupEnded = false;
            int nextColumn = printArea.Start.Column;
            if(startColumns.Count <= 1)
            {
                startColumns = GetStartGroupColumns(cells, row, printArea);                
                nextColumn = GetNextColumn(printArea, startColumns, nextColumn);
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
                    lastGroup.Cells.Add(cell);
                    continue;
                }
                int column = cell.Address.Column;
                if(column >= nextColumn)
                {                    
                    GroupedCells? nextGroup = results.FirstOrDefault(g => g.StartColumn == column);
                    if(nextGroup == null)
                    {
                        nextGroup = new ()
                        {
                            StartColumn = column,
                            DisplayOrder = results.Count,
                        };
                        results.Add(nextGroup);
                        lastGroup = nextGroup;
                    }
                    else
                    {
                        lastGroup = nextGroup;
                    }
                    nextColumn = GetNextColumn(printArea, startColumns, nextColumn);
                }
                lastGroup.Cells.Add(cell);
            }
            if(groupEnded)
            {
                groupEnded = false;
                startColumns.Clear();
                nextColumn = printArea.Start.Column;
                GroupedCells nextGroup = new ()
                {
                    StartColumn = printArea.Start.Column,
                    DisplayOrder = results.Count,
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
    private static int GetNextColumn(Worksheets.PrintArea printArea, List<int> startColumns, int nextColumn)
    {
        int result = nextColumn;
        int lastColumn = nextColumn;
        foreach(int c in startColumns)
        {
            if(result < c)
            {
                result = c;
                break;
            }
        }
        if(lastColumn == result)
        {
            result = printArea.End.Column + 1;
        }
        return result;
    }
}
