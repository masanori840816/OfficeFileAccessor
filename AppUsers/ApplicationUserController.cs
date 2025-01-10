using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeFileAccessor.AppUsers.DTO;

namespace OfficeFileAccessor.AppUsers;

public class ApplicationUserController(ApplicationUserService Users): Controller
{
    [AllowAnonymous]
    [HttpPost("/api/users/signin")]
    public async Task<IActionResult> ApplicationSignIn([FromForm] SignInValue value)
    {
        return Json(await Users.SignInAsync(value, HttpContext.Session));
    }
    [HttpGet("/api/users/signout")]
    public async Task ApplicationSignOut()
    {
        await Users.SignOutAsync();
    }
}