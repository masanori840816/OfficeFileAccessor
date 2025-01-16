using OfficeFileAccessor.Apps;
using OfficeFileAccessor.AppUsers.DTO;

namespace OfficeFileAccessor.AppUsers;

public interface IApplicationUserService
{
    Task<ApplicationResult> SignInAsync(SignInValue value, ISession session);
    Task SignOutAsync();
}