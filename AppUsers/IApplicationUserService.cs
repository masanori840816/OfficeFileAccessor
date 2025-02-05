using System.Security.Claims;

namespace OfficeFileAccessor.AppUsers;

public interface IApplicationUserService
{
    Task<DTO.SignInResult> SignInAsync(DTO.SignInValue value, HttpResponse response);
    Task SignOutAsync(HttpResponse response);
    Task<DTO.DisplayUser?> GetSignedInUserAsync(ClaimsPrincipal? user);
}