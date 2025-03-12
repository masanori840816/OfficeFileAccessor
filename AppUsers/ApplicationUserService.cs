using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using OfficeFileAccessor.Apps;
using OfficeFileAccessor.AppUsers.Entities;
using OfficeFileAccessor.AppUsers.Repositories;
using OfficeFileAccessor.Web;

namespace OfficeFileAccessor.AppUsers;

public class ApplicationUserService(SignInManager<ApplicationUser> SignIn,
    IApplicationUsers Users,
    IUserTokens Tokens): IApplicationUserService
{
    public async Task<DTO.SignInResult> SignInAsync(DTO.SignInValue value, HttpResponse response)
    {
        var target = await Users.GetByEmailForSignInAsync(value.Email);
        if(target == null)
        {
            return new (Result: ApplicationResult.GetFailedResult("Invalid e-mail or password"), User: null);
        }
        SignInResult result = await SignIn.PasswordSignInAsync(target, value.Password, false, false);
        if(result.Succeeded)
        {
            response.Cookies.Append("User-Token", Tokens.GenerateToken(target), DefaultCookieOption.Get());         
            return new (Result: ApplicationResult.GetSucceededResult(), User: DTO.DisplayUser.Create(target));
        }
        return new (Result: ApplicationResult.GetFailedResult("Invalid e-mail or password"), User: null);
    }
    public async Task SignOutAsync(HttpResponse response)
    {
        await SignIn.SignOutAsync();
        response.Cookies.Delete("User-Token");     
    }
    public async Task<DTO.DisplayUser?> GetSignedInUserAsync(ClaimsPrincipal? user)
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
        return DTO.DisplayUser.Create(signedInUser);
    }
    public async Task<List<DTO.SearchUser>> SearchUsersAsync(string? organization, string? userName,
        string? email, string? updateDateFrom, string? updateDateTo)
    {
        return await Users.SearchUsersAsync(organization, userName, email,
            updateDateFrom, updateDateTo);
    }
    public async Task<DTO.DisplayUser?> GetUserAsync(int userId)
    {
        ApplicationUser? user = await Users.GetUserByIdAsync(userId);
        if(user == null)
        {
            return null;
        }
        return DTO.DisplayUser.Create(user);
    }
    public async Task<ApplicationResult> UpdateUserAsync(DTO.UpdateUser user)
    {
        if(string.IsNullOrEmpty(user.UserName))
        {
            return ApplicationResult.GetFailedResult("UserName is required");
        }
        if(string.IsNullOrEmpty(user.Email))
        {
            return ApplicationResult.GetFailedResult("Email is required");
        }
        if(string.IsNullOrEmpty(user.Password))
        {
            return ApplicationResult.GetFailedResult("Password is required");
        }
        return await Users.CreateOrUpdateUserAsync(user);
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