
namespace OfficeFileAccessor.OfficeFiles.Worksheets;

public class Cell
{
    public required CellAddress Address { get; init; }
    public required CellValueType Type { get; init; }
    public required string Value { get; init; }
    public string? Formula { get; init; }
    public required double Width { get; init; }
    public required double Height { get; init; }
    public string? BackgroundColor { get; init; }
    public required CellBorders Borders { get; init; }
    // Only the start cell of a merged cell has this value.
    public MergedCell? MergedCell { get; init; }
    public bool Merged { get; init; }

    public override string ToString()
    {
        return $"Cell Address:{Address} Type:{Type} Formula:{Formula} Value:{Value} W: {Width} H: {Height} BackgroundColor: {BackgroundColor} {Borders} Merged?: {Merged} {MergedCell}";
    }
}