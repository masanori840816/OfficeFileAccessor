
using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles.Worksheets;

public class Cell
{
    public required CellAddress Address { get; init; }
    public required string Value { get; init; }
    public string? Formula { get; init; }
    public string? BackgroundColor { get; init; }
    public TableCellFontFormat? FontFormat { get; init; }
    public required TableCellBorders Borders { get; init; }
    // Only the start cell of a merged cell has this value.
    public MergedCell? MergedCell { get; init; }
    public bool Merged { get; init; }
    public required bool VerticalWriting { get; init; }
    public required uint TextRotation { get; init; }

    public override string ToString()
    {
        return $"Cell Address:{Address} Formula:{Formula} Value:{Value} BackgroundColor: {BackgroundColor} {FontFormat} {Borders} Merged?: {Merged} {MergedCell} VerticalWriting?: {VerticalWriting} TextRotation: {TextRotation}";
    }
    public static Cell Default(string? address)
    {
        return new ()
        {
            Address = CellAddress.GenerateFromAddress(address),
            Value = "",
            Borders = TableCellBorders.GetNoBorders(),
            VerticalWriting = false,
            TextRotation = 0
        };
    }
}