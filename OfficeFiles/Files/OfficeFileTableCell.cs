
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
}