using OfficeFileAccessor.AppUsers.Entities;

namespace OfficeFileAccessor.AppUsers.Repositories;

public interface IApplicationUsers
{
    Task<ApplicationUser?> GetByEmailForSigninAsync(string email);
}