using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeFileAccessor.AppUsers;
using OfficeFileAccessor.AppUsers.DTO;

namespace OfficeFileAccessor.OfficeFiles;

[AutoValidateAntiforgeryToken]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OfficeFileController(ILogger<OfficeFileController> logger, IOfficeFileService officeFiles,
    IApplicationUserService Users): Controller
{
    [HttpGet("/api/files")]
    public string GetFileNames()
    {
        logger.LogInformation("GetFileNames");
        return "Viva Las Vegas!";
    }
    [HttpPost("/api/files")]
    public async Task<IActionResult> LoadOfficeFiles([FromForm] IFormFileCollection files)
    {
        DisplayUser? user = await Users.GetSignedInUserAsync(User);
        logger.LogInformation("Sign-in User: {user}", user);
        return Json(await officeFiles.RegisterAsync(files));
    }
}