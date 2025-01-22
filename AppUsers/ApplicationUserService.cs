using System.Security.Claims;
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
    public async Task<DisplayUser?> GetSignedInUserAsync(ClaimsPrincipal? user)
    {
        string? email = GetSignedInUserEmail(user);
        if(string.IsNullOrEmpty(email))
        {
            return null;
        }
        ApplicationUser? signedInUser = await Users.GetByEmailForSignInAsync(email);
        if(signedInUser == null)
        {
            return null;
        }
        return DisplayUser.Create(signedInUser);
    }
    /// <summary>
    /// Get signed in user email address
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private static string? GetSignedInUserEmail(ClaimsPrincipal? user)
    {
        if(user == null)
        {
            return null;
        }
        if(user.Identity?.IsAuthenticated != true)
        {
            return null;
        }        
        return user.FindFirstValue(ClaimTypes.Email);
    }
}