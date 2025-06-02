using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
    [JsonIgnore]
    public DateTime LastUpdateDate { get; init; }
    public string UpdateDateText => $"{LastUpdateDate.ToLocalTime().ToShortDateString()} {LastUpdateDate.ToLocalTime().ToLongTimeString()}";
}