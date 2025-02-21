
namespace OfficeFileAccessor.OfficeFiles.Worksheets;

public class Cell
{
    public required CellAddress Address { get; init; }
    public required CellValueType Type { get; init; }
    public required string Value { get; init; }
    public string? Formula { get; init; }
    public string? BackgroundColor { get; init; }
    public CellFontFormat? FontFormat { get; init; }
    public required CellBorders Borders { get; init; }
    // Only the start cell of a merged cell has this value.
    public MergedCell? MergedCell { get; init; }
    public bool Merged { get; init; }

    public override string ToString()
    {
        return $"Cell Address:{Address} Type:{Type} Formula:{Formula} Value:{Value} BackgroundColor: {BackgroundColor} {FontFormat} {Borders} Merged?: {Merged} {MergedCell}";
    }
    public static Cell Default(string? address)
    {
        return new ()
        {
            Address = CellAddress.GenerateFromAddress(address),
            Type = CellValueType.Text,
            Value = "",
            Borders = CellBorders.GetNoBorders(),
        };
    }
}