namespace OfficeFileAccessor.OfficeFiles.Files;

public class OfficeFileGenerator(ILogger<OfficeFileGenerator> Logger): IOfficeFileGenerator
{
    private record TitleCellAddresses(string Title, List<Worksheets.CellAddress> Addresses);
    private record GroupedCells
    {
        public required int StartColumn { get; init; }
        public required int DisplayOrder { get; init; }
        public required bool Ended { get; set; }
        public List<Worksheets.Cell> Cells { get; init; } = [];
    }
    public List<OfficeFileTableGroup> Generate(string sheetName, Worksheets.PrintArea printArea,
        List<Worksheets.Cell> cells)
    {
        List<GroupedCells> groupedCells = GroupCells(printArea, cells);
        Worksheets.CellBorders noBorders = Worksheets.CellBorders.GetNoBorders();
        Worksheets.CellBorders allThin = Worksheets.CellBorders.GetAllThin();        
        List<OfficeFileTableGroup> results = [];
        foreach(GroupedCells g in groupedCells)
        {
            OfficeFileTableGroup group = new ()
            {
                DisplayOrder = results.Count,
            };
            results.Add(group);
            List<Worksheets.CellAddress> addedAddresses = [];
            List<OfficeFileTableCell> tableCells = [];
            
            if(g.Cells.Any(c => c.Borders.CheckIsBordered()) == false)
            {
                if(results.Count <= 1)
                {
                    TitleCellAddresses? titles = null;
                    foreach(Worksheets.Cell c in g.Cells)
                    {
                        if(titles == null)
                        {
                            titles = GetTitle(c, g.Cells);
                            if(titles != null)
                            {
                                group.Title = titles.Title;
                                continue;
                            }
                        }
                        if(string.IsNullOrEmpty(c.Value) == false &&
                            (titles == null || titles.Addresses.Any(a => a == c.Address) == false))
                        {
                            group.Cells.Add(OfficeFileTableCell.Generate(c));
                        }
                    }
                }
                else
                {
                    foreach(Worksheets.Cell c in g.Cells)
                    {
                        if(string.IsNullOrEmpty(c.Value) == false)
                        {
                            group.Cells.Add(OfficeFileTableCell.Generate(c));
                        }
                    }
                }
                continue;
            }
            foreach(Worksheets.Cell cell in g.Cells)
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
                List<Worksheets.Cell> mergedCell = [cell];
                AddMergedCells(cell, mergedCell, g.Cells);
                AddRightTop(cell.Address, mergedCell, g.Cells);
                AddLeftBottom(cell.Address, mergedCell, g.Cells);
                int[] columns = [.. mergedCell.Select(c => c.Address.Column).Distinct()];
                int[] rows = [.. mergedCell.Select(c => c.Address.Row).Distinct()];
                AddRestCells(mergedCell, g.Cells, columns, rows);
                string? backgroundColor = null;
                foreach(Worksheets.Cell c in mergedCell)
                {
                    if(c.BackgroundColor == "FFFF00")
                    {
                        backgroundColor = c.BackgroundColor;
                        break;
                    }
                    if(string.IsNullOrEmpty(c.BackgroundColor) == false)
                    {
                        backgroundColor = c.BackgroundColor;
                    }
                }
                addedAddresses.AddRange(mergedCell.Select(c => c.Address));
                group.Cells.Add(
                    OfficeFileTableCell.Generate(cell.Address, MergeCellValues(mergedCell), allThin, 
                        backgroundColor, Worksheets.MergedCell.Generate(mergedCell)));
            }
        }
        
        return results;
    }
    private static TitleCellAddresses? GetTitle(Worksheets.Cell cell, List<Worksheets.Cell> cells)
    {
        if(string.IsNullOrEmpty(cell.Value))
        {
            return null;
        }
        if(cell.FontFormat?.Bold != true)
        {
            return null;
        }
        string title = cell.Value;
        List<Worksheets.CellAddress> addresses = [cell.Address];
        int offset = 1;
        while(true)
        {
            Worksheets.CellAddress next = Worksheets.CellAddress.Move(cell.Address, offset, 0);
            Worksheets.Cell? nextCell = cells.FirstOrDefault(c => c.Address == next);
            if(string.IsNullOrEmpty(nextCell?.Value))
            {
                break;
            }
            title += nextCell.Value;
            addresses.Add(next);
            offset += 1;
        }
        return new (Title: title, Addresses: addresses);
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
    private static void AddRightTop(Worksheets.CellAddress baseAddress,
        List<Worksheets.Cell> current, List<Worksheets.Cell> allCells)
    {
        if(current.Any(ce => ce.Borders.Right != Worksheets.BorderType.None &&
                ce.Borders.Top != Worksheets.BorderType.None))
        {
            return;
        }
        Worksheets.CellAddress rightAddress = Worksheets.CellAddress.Move(baseAddress, 1, 0);
        Worksheets.Cell? right = allCells.FirstOrDefault(c => c.Address == rightAddress);
        if(right == null)
        {
            return;
        }
        current.Add(right);
        AddMergedCells(right, current, allCells);
        AddRightTop(rightAddress, current, allCells);
    }
    private static void AddLeftBottom(Worksheets.CellAddress baseAddress,
        List<Worksheets.Cell> current, List<Worksheets.Cell> allCells)
    {
        if(current.Any(ce => ce.Borders.Bottom != Worksheets.BorderType.None))
        {
            return;
        }
        Worksheets.CellAddress bottomAddress = Worksheets.CellAddress.Move(baseAddress, 0, 1);
        Worksheets.Cell? bottom = allCells.FirstOrDefault(c => c.Address == bottomAddress);
        if(bottom == null || bottom.Borders.Top != Worksheets.BorderType.None)
        {
            return;
        }
        current.Add(bottom);
        AddMergedCells(bottom, current, allCells);
        AddLeftBottom(bottomAddress, current, allCells);
    }
    private static string MergeCellValues(List<Worksheets.Cell> cells)
    {
        bool first = true;
        int startColumn = cells.Min(c => c.Address.Column);
        int lastRow = -1;
        string result = "";
        string currentRowText = "";
        foreach(Worksheets.Cell cell in cells.OrderBy(ce => ce.Address.Row).ThenBy(ce => ce.Address.Column))
        {
            if(first)
            {
                lastRow = cell.Address.Row;
                first = false;
            }
            if(lastRow != cell.Address.Row)
            {
                if(string.IsNullOrEmpty(result) == false)
                {
                    result += "[NEW-LINE]";
                }
                if(string.IsNullOrEmpty(currentRowText) == false)
                {
                    result += currentRowText;
                    currentRowText = "";
                }
                lastRow = cell.Address.Row;
            }
            if(string.IsNullOrEmpty(cell.Value) == false)
            {
                if(string.IsNullOrEmpty(currentRowText))
                {
                    for(int i = 0; i < (cell.Address.Column - startColumn); i++)
                    {
                        currentRowText += "[TAB]";
                    }
                }
                currentRowText += cell.Value;
                currentRowText += " ";
            }
        }        
        if(string.IsNullOrEmpty(currentRowText) == false)
        {        
            if(string.IsNullOrEmpty(result) == false)
            {
                result += "[NEW-LINE]";
            }
            result += currentRowText;
        }
        result = result.Replace("\r\n", "[NEW-LINE]");
        return result.Replace("\n", "[NEW-LINE]");
    }
    private static void AddRestCells(List<Worksheets.Cell> current, List<Worksheets.Cell> allCells,
        int[] columns, int[] rows)
    {
        foreach(Worksheets.Cell cell in allCells.Where(c => columns.Contains(c.Address.Column) && rows.Contains(c.Address.Row)))
        {
            if(current.Any(cu => cu.Address == cell.Address) == false)
            {
                current.Add(cell);
            }
        }
    }
    private static List<GroupedCells> GroupCells(Worksheets.PrintArea printArea, List<Worksheets.Cell> cells)
    {
        Worksheets.Cell[] ordered = [.. cells.OrderBy(c => c.Address.Row).ThenBy(c => c.Address.Column)];
        List<GroupedCells> results = [];
        GroupedCells? lastGroup = null;
        List<int> startColumns = [];
        for(int row = printArea.Start.Row; row <= printArea.End.Row; row++)
        {
            bool groupEnded = false;
            int nextColumn = printArea.Start.Column;
            if(startColumns.Count <= 1)
            {
                startColumns = GetStartGroupColumns(cells, row, printArea);                
                nextColumn = GetNextColumn(printArea, startColumns, nextColumn);
                if(startColumns.Count > 1)
                {
                    if(lastGroup != null)
                    {
                        lastGroup.Ended = true;
                    }
                    lastGroup = null;
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
                    lastGroup = GetOrCreateGroup(lastGroup, cell, results);
                    lastGroup.Cells.Add(cell);
                    continue;
                }
                int column = cell.Address.Column;
                if(column >= nextColumn)
                {                    
                    GroupedCells? nextGroup = results.FirstOrDefault(g => g.Ended == false && g.StartColumn == column);
                    if(nextGroup == null)
                    {
                        nextGroup = new ()
                        {
                            StartColumn = column,
                            DisplayOrder = results.Count,
                            Ended = false,
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
                lastGroup = GetOrCreateGroup(lastGroup, cell, results);
                lastGroup.Cells.Add(cell);
            }
            if(groupEnded)
            {
                groupEnded = false;
                startColumns.Clear();
                lastGroup = null;
                for(int i = 0; i < results.Count; i++)
                {
                    if(results[i].Cells.Any(c => c.Address.Row == row))
                    {
                        results[i].Ended = true;
                    }
                }
            }
        }
        return results;
    }
    private static GroupedCells GetOrCreateGroup(GroupedCells? currentGroup, Worksheets.Cell currentCell, List<GroupedCells> groupedCells)
    {
        if(currentGroup == null)
        {
            currentGroup = new ()
            {
                StartColumn = currentCell.Address.Column,
                DisplayOrder = groupedCells.Count,
                Ended = false,
            };
            groupedCells.Add(currentGroup);
        }
        return currentGroup;
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
                    results.Add(cells[i].Address.Column + 1);
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
