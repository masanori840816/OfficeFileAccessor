using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OfficeFileAccessor.OfficeFiles;
public class OfficeFileController(ILogger<OfficeFileController> logger, IOfficeFileService officeFiles): Controller
{
    [AutoValidateAntiforgeryToken]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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