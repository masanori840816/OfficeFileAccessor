using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeFileAccessor.AppUsers.DTO;

namespace OfficeFileAccessor.AppUsers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ApplicationUserController(IApplicationUserService Users): Controller
{
    [AutoValidateAntiforgeryToken]
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
    [AutoValidateAntiforgeryToken]
    [HttpGet("/api/auth")]
    public IActionResult CheckAuthenticationStatus()
    {
        return Ok();
    }
}