
namespace OfficeFileAccessor.OfficeFiles.Files;

public record OfficeFileTableGroup
{
    public long? Id { get; init; }
    public long OfficeFileId { get; init; }
    public required int DisplayOrder { get; init; }
    public required string SheetName { get; init; }
    public string? Title { get; set; }
    public List<OfficeFileTableCell> Cells { get; init; } = [];
}