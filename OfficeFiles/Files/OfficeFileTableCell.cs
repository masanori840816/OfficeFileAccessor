
using OfficeFileAccessor.OfficeFiles.Worksheets;

namespace OfficeFileAccessor.OfficeFiles.Files;

public record OfficeFileTableCell
{
    public long? Id { get; init; }
    public required CellAddress CellAddress { get; init; }

    public int VerticalLength { get; init; } = 1;
    public int HorizontalLength { get; init;} = 1;
    public required string Value { get; init; }
    public required double Width { get; init; }
    public required double Height { get; init; }
    public required CellBorders Borders { get; init; }
    public string? BackgroundColor { get; init; }
    public bool Editabled { get; init; }
    public static OfficeFileTableCell Generate(Cell cell, List<OfficeFileTableColumnWidth> widths, List<OfficeFileTableRowHeight> heights)
    {
        int horizontalLength = 1;
        int verticalLength = 1;
        double width = 0d;
        double height = 0d;
        if(cell.MergedCell == null)
        {
            width = cell.Width;
            height = cell.Height;
        }
        else
        {
            horizontalLength = cell.MergedCell.End.Column - cell.MergedCell.Start.Column + 1;
            verticalLength = cell.MergedCell.End.Row - cell.MergedCell.Start.Row + 1;
            foreach(OfficeFileTableColumnWidth w in widths)
            {
                if(cell.MergedCell.Start.Column <= w.Column && cell.MergedCell.End.Column >= w.Column)
                {
                    width += w.Width;
                }
            }
            foreach(OfficeFileTableRowHeight h in heights)
            {
                if(cell.MergedCell.Start.Row <= h.Row && cell.MergedCell.End.Row >= h.Row)
                {
                    height += h.Height;
                }
            }
        }

        return new ()
        {
            CellAddress = cell.Address,
            HorizontalLength = horizontalLength,
            VerticalLength = verticalLength,
            Value = cell.Value,
            Width = width,
            Height = height,
            Borders = cell.Borders,
            BackgroundColor = cell.BackgroundColor,
            Editabled = cell.BackgroundColor == "FFFF00"
        };
    }
    public static OfficeFileTableCell Generate(CellAddress baseAddress, string mergedValue, CellBorders borders,
        string? backgroundColor, MergedCell? mergedCell, double mergedWidth, double mergedHeight)
    {
        int horizontalLength = 1;
        int verticalLength = 1;
        if(mergedCell != null)
        {
            horizontalLength = mergedCell.End.Column - mergedCell.Start.Column + 1;
            verticalLength = mergedCell.End.Row - mergedCell.Start.Row + 1;
        }
        return new ()
        {
            CellAddress = baseAddress,
            HorizontalLength = horizontalLength,
            VerticalLength = verticalLength,
            Value = mergedValue,
            Width = mergedWidth,
            Height = mergedHeight,
            Borders = borders,
            BackgroundColor = backgroundColor,
            Editabled = backgroundColor == "FFFF00"
        };
    }
}