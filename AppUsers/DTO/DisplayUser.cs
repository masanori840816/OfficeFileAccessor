using OfficeFileAccessor.AppUsers.Entities;

namespace OfficeFileAccessor.AppUsers.DTO;

/// <summary>
/// ApplicationUser for display.
/// </summary>
/// <param name="Id"></param>
/// <param name="UserName"></param>
/// <param name="Organization"></param>
/// <param name="Email"></param>
public record DisplayUser
{
    public int Id { get; init; }
    public string? UserName { get; init; }
    public string? Organization { get; init; }
    public string? Email { get; init; }

    public static DisplayUser Create(ApplicationUser user)
    {
        return new ()
        {
            Id = user.Id,
            UserName = user.UserName,
            Organization = user.Organization,
            Email = user.Email,
        };
    }
}