using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeFileAccessor.AppUsers.DTO;

namespace OfficeFileAccessor.AppUsers;

[AutoValidateAntiforgeryToken]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ApplicationUserController(IAntiforgery Antiforgery, IApplicationUserService Users): Controller
{
    [AllowAnonymous]
    [HttpPost("/api/users/signin")]
    public async Task<IActionResult> ApplicationSignIn([FromBody] SignInValue value)
    {
        return Json(await Users.SignInAsync(value, Response));
    }
    [HttpGet("/api/users/signout")]
    public async Task ApplicationSignOut()
    {
        await Users.SignOutAsync(Response);
    }
    [HttpGet("/api/auth")]
    public IActionResult CheckAuthenticationStatus()
    {
        var tokenSet = Antiforgery.GetAndStoreTokens(HttpContext);
        if(tokenSet?.RequestToken != null) {
            HttpContext.Response.Cookies.Append("XSRF-TOKEN", tokenSet.RequestToken,
            new CookieOptions { 
                HttpOnly = false,
                SameSite = SameSiteMode.Lax,
            });
        }
        return Ok();
    }
}