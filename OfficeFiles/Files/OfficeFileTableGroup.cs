
using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles.Files;

public record OfficeFileTableGroup
{
    public long? Id { get; init; }
    public long SheetId { get; init; }
    public required int DisplayOrder { get; init; }
    public string? Title { get; set; }
    public List<TableCell> Cells { get; init; } = [];
}