using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeFileAccessor.AppUsers;

namespace OfficeFileAccessor.OfficeFiles;
public class OfficeFileController(ILogger<OfficeFileController> logger, IOfficeFileService officeFiles): Controller
{
    [Authorize(AuthenticationSchemes = UserTokens.AuthSchemes)]
    [HttpGet("/api/files")]
    public string GetFileNames()
    {
        logger.LogInformation("GetFileNames");
        return "Viva Las Vegas!";
    }
    [HttpPost("/api/files")]
    public async Task<IActionResult> LoadOfficeFiles([FromForm] IFormFileCollection files)
    {
        
        return Json(await officeFiles.RegisterAsync(files));
    }
}