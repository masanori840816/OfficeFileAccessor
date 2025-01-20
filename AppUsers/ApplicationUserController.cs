using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeFileAccessor.AppUsers.DTO;

namespace OfficeFileAccessor.AppUsers;

[AutoValidateAntiforgeryToken]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ApplicationUserController(IApplicationUserService Users): Controller
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
        return Ok();
    }
}