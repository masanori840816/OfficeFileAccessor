using System.Security.Claims;
using OfficeFileAccessor.Apps;
using OfficeFileAccessor.AppUsers.DTO;

namespace OfficeFileAccessor.AppUsers;

public interface IApplicationUserService
{
    Task<DTO.SignInResult> SignInAsync(DTO.SignInValue value, HttpResponse response);
    Task SignOutAsync(HttpResponse response);
    Task<DTO.DisplayUser?> GetSignedInUserAsync(ClaimsPrincipal? user);
    Task<List<SearchUser>> SearchUsersAsync(string? organization, string? userName,
        string? email, string? updateDateFrom, string? updateDateTo);
    Task<DisplayUser?> GetUserAsync(int userId);
    Task<ApplicationResult> UpdateUserAsync(UpdateUser user);
}