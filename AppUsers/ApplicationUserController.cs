using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeFileAccessor.AppUsers.DTO;

namespace OfficeFileAccessor.AppUsers;

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
        await Users.SignOutAsync();
    }
}