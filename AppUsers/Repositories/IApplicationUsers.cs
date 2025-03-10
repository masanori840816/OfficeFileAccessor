using OfficeFileAccessor.AppUsers.DTO;
using OfficeFileAccessor.AppUsers.Entities;

namespace OfficeFileAccessor.AppUsers.Repositories;

public interface IApplicationUsers
{
    Task<ApplicationUser?> GetByEmailForSignInAsync(string email);
    Task<List<SearchUser>> SearchUsersAsync(string? organization, string? userName,
        string? updateDateFrom, string? updateDateTo);
}