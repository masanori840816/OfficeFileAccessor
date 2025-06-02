using OfficeFileAccessor.AppUsers.DTO;

namespace OfficeFileAccessor.Logs;

public record LogValue
{
    public required string LoggedDate { get; init; }
    public required int UserId { get; init; }
    public required string Page { get; init; }
    public required string Action { get; init; }
    public string? Contents { get; init; }
    public static LogValue LogPageOpen(DisplayUser user, string page)
    {
        return new ()
        {
            LoggedDate = DateTime.Now.ToString("yyyyMMdd HH:mm:ss"),
            UserId = user.Id,
            Page = page,
            Action = "open"
        };
    }
}