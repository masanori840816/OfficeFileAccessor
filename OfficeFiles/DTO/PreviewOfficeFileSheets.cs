using System.ComponentModel.DataAnnotations;

namespace OfficeFileAccessor.OfficeFiles.DTO;

public record PreviewOfficeFileSheets
{
    public required long FileId { get; init; }
    public required string FileName { get; init; }
    [Key]
    public required long SheetId { get; init; }
    public required string SheetName { get; init; }
    public int DisplayOrder { get; init; }
    public required string RegisterUser { get; init; }
}