
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
    public static OfficeFileTableCell Generate(Cell cell)
    {
        return new ()
        {
            CellAddress = cell.Address,
            HorizontalLength = 1,
            VerticalLength = 1,
            Value = cell.Value,
            Width = cell.Width,
            Height = cell.Height,
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