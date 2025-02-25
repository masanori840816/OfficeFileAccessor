
using OfficeFileAccessor.OfficeFiles.Worksheets;

namespace OfficeFileAccessor.OfficeFiles.Files;

public record OfficeFileTableCell
{
    public long? Id { get; init; }
    public required CellAddress CellAddress { get; init; }
    public CellFontFormat? FontFormat { get; init; }
    public int VerticalLength { get; set; } = 1;
    public int HorizontalLength { get; set;} = 1;
    public required string Value { get; init; }
    public required CellBorders Borders { get; init; }
    public string? BackgroundColor { get; init; }
    public bool Editabled { get; init; }
    public MergedCell? MergedCell { get; init; }
    public static OfficeFileTableCell Generate(Cell cell)
    {
        int horizontalLength = 1;
        int verticalLength = 1;
        
        return new ()
        {
            CellAddress = cell.Address,
            FontFormat = cell.FontFormat,
            HorizontalLength = horizontalLength,
            VerticalLength = verticalLength,
            Value = cell.Value,
            Borders = cell.Borders,
            BackgroundColor = cell.BackgroundColor,
            Editabled = cell.BackgroundColor == "FFFF00"
        };
    }
    public static OfficeFileTableCell Generate(CellAddress baseAddress, string mergedValue, CellBorders borders,
        string? backgroundColor, MergedCell? mergedCell)
    {
        return new ()
        {
            CellAddress = baseAddress,
            Value = mergedValue,
            Borders = borders,
            BackgroundColor = backgroundColor,
            Editabled = backgroundColor == "FFFF00",
            MergedCell = mergedCell,
        };
    }
    public void UpdateCellLength(List<OfficeFileTableColumnWidth> widths, List<OfficeFileTableRowHeight> heights)
    {
        if(MergedCell == null)
        {
            return;
        }
        int horizontalLength = 0;
        int verticalLength = 0;
        foreach(OfficeFileTableColumnWidth w in widths)
        {
            if(MergedCell.Start.Column <= w.Column && MergedCell.End.Column >= w.Column)
            {
                horizontalLength += 1;
            }
        }
        foreach(OfficeFileTableRowHeight h in heights)
        {
            if(MergedCell.Start.Row <= h.Row && MergedCell.End.Row >= h.Row)
            {
                verticalLength += 1;
            }
        }
        HorizontalLength = horizontalLength;
        VerticalLength = verticalLength;
    }
}