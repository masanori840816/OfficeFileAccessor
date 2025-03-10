using System.Text.Json.Serialization;

namespace OfficeFileAccessor.AppUsers.DTO;

public record SearchUser
{
    public int Id { get; init; }
    public required string UserName { get; init; }
    public string? Organization { get; init; }
    public required string Email { get; init; }
    public int UseCount { get; init;}
    [JsonIgnore]
    public DateTime LastUpdateDate { get; init; }
    public string UpdateDateText => $"{LastUpdateDate.ToLocalTime().ToShortDateString()} {LastUpdateDate.ToLocalTime().ToLongTimeString()}";
}