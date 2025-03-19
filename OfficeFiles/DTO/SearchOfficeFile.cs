
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OfficeFileAccessor.OfficeFiles.DTO;

public record SearchOfficeFile
{
    public required long FileId { get; init; }
    public required string FileName { get; init; }
    public required string UserName { get; init; }
    [JsonIgnore]
    public int RegisterUserId { get; init; }
    public required int UseCount { get; init; }
    public int Version { get; init; }
    [JsonIgnore]
    public DateTime LastUpdateDate { get; init; }
    [NotMapped]
    public string UpdateDateText => LastUpdateDate.ToLocalTime().ToString("yyyy-MM-dd");
}