namespace OfficeFileAccessor.OfficeFiles.Files;

public record OfficeFileSheet
{
    public long? Id { get; init; }
    public long OfficeFileId { get; init; }
    public required string Name { get; init; }
    public List<OfficeFileTableGroup> TableGroups { get; init; } = [];
    public required List<OfficeFileTableColumnWidth> Widths { get; init; }
    public required List<OfficeFileTableRowHeight> Heights { get; init; }
}