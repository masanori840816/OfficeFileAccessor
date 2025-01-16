using Microsoft.AspNetCore.Identity;
using OfficeFileAccessor.Apps;
using OfficeFileAccessor.AppUsers.DTO;
using OfficeFileAccessor.AppUsers.Entities;
using OfficeFileAccessor.AppUsers.Repositories;

namespace OfficeFileAccessor.AppUsers;

public class ApplicationUserService(SignInManager<ApplicationUser> SignIn,
    IApplicationUsers Users,
    IUserTokens Tokens): IApplicationUserService
{
    public async Task<ApplicationResult> SignInAsync(SignInValue value, ISession session)
    {
        var target = await Users.GetByEmailForSignInAsync(value.Email);
        if(target == null)
        {
            return ApplicationResult.GetFailedResult("Invalid e-mail or password");
        }
        SignInResult result = await SignIn.PasswordSignInAsync(target, value.Password, false, false);
        if(result.Succeeded)
        {
            session.SetString("User-Token", Tokens.GenerateToken(target));         
            return ApplicationResult.GetSucceededResult();
        }
        return ApplicationResult.GetFailedResult("Invalid e-mail or password");
    }
    public async Task SignOutAsync()
    {
        await SignIn.SignOutAsync();
    }
}