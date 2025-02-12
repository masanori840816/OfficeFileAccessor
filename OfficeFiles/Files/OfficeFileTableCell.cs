
using OfficeFileAccessor.OfficeFiles.Worksheets;

namespace OfficeFileAccessor.OfficeFiles.Files;

public record OfficeFileTableCell
{
    public long? Id { get; init; }
    public required CellAddress CellAddress { get; init; }

    public int VerticalLength { get; init; } = 1;
    public int HorizontalLength { get; init;} = 1;
    public required string ValueType { get; init; }
    public required string Value { get; init; }
    public string? Formula { get; init; }
    public required CellBorders Borders { get; init; }
    public string? BackgroundColor { get; init; }
    public bool Editabled { get; init; }

    public static OfficeFileTableCell Create(Worksheets.Cell cell)
    {
        int horizontalLength = 1;
        int verticalLength = 1;
        if(cell.Merged && cell.MergedCell != null)
        {
            horizontalLength = cell.MergedCell.End.Column - cell.MergedCell.Start.Column + 1;
            verticalLength = cell.MergedCell.End.Row - cell.MergedCell.Start.Row + 1;
        }

        return new ()
        {
            CellAddress = cell.Address,
            HorizontalLength = horizontalLength,
            VerticalLength = verticalLength,
            ValueType = cell.Type.ToString(),
            Value = cell.Value,
            Formula = cell.Formula,
            Borders = cell.Borders,
            BackgroundColor = cell.BackgroundColor,
            Editabled = cell.BackgroundColor == "FFFF00"
        };
    }
}