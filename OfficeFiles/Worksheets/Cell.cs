
namespace OfficeFileAccessor.OfficeFiles.Worksheets;

public class Cell
{
    public required string Address { get; init; }
    public required CellValueType Type { get; init; }
    public required string Value { get; init; }
    public string? Formula { get; init; }

    public override string ToString()
    {
        return $"Cell Address:{Address} Type:{Type} Formula:{Formula} Value:{Value}";
    }
}