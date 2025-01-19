using Microsoft.AspNetCore.Identity;
using OfficeFileAccessor.Apps;
using OfficeFileAccessor.AppUsers.DTO;
using OfficeFileAccessor.AppUsers.Entities;
using OfficeFileAccessor.AppUsers.Repositories;
using OfficeFileAccessor.Web;

namespace OfficeFileAccessor.AppUsers;

public class ApplicationUserService(SignInManager<ApplicationUser> SignIn,
    IApplicationUsers Users,
    IUserTokens Tokens): IApplicationUserService
{
    public async Task<ApplicationResult> SignInAsync(SignInValue value, HttpResponse response)
    {
        var target = await Users.GetByEmailForSignInAsync(value.Email);
        if(target == null)
        {
            return ApplicationResult.GetFailedResult("Invalid e-mail or password");
        }
        SignInResult result = await SignIn.PasswordSignInAsync(target, value.Password, false, false);
        if(result.Succeeded)
        {
            response.Cookies.Append("User-Token", Tokens.GenerateToken(target), DefaultCookieOption.Get());         
            return ApplicationResult.GetSucceededResult();
        }
        return ApplicationResult.GetFailedResult("Invalid e-mail or password");
    }
    public async Task SignOutAsync(HttpResponse response)
    {
        await SignIn.SignOutAsync();
        response.Cookies.Delete("User-Token");     
    }
}