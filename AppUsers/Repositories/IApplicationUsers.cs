using OfficeFileAccessor.Apps;
using OfficeFileAccessor.AppUsers.DTO;
using OfficeFileAccessor.AppUsers.Entities;

namespace OfficeFileAccessor.AppUsers.Repositories;

public interface IApplicationUsers
{
    Task<ApplicationUser?> GetByEmailForSignInAsync(string email);
    Task<ApplicationUser?> GetUserByIdAsync(int userId);
    Task<List<SearchUser>> SearchUsersAsync(string? organization, string? userName,
        string? email, string? updateDateFrom, string? updateDateTo);
    Task<ApplicationResult> CreateOrUpdateUserAsync(UpdateUser user);
}