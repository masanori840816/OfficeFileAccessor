using OfficeFileAccessor.AppUsers.Entities;

namespace OfficeFileAccessor.AppUsers;

public interface IUserTokens
{
    string GenerateToken(ApplicationUser user);
}