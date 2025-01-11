using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OfficeFileAccessor.OfficeFiles;
public class OfficeFileController(ILogger<OfficeFileController> logger, IOfficeFileService officeFiles): Controller
{
    [Authorize]
    [HttpGet("/api/files")]
    public string GetFileNames()
    {
        logger.LogInformation("GetFileNames");
        return "Hello World!";
    }
    [HttpPost("/api/files")]
    public async Task<IActionResult> LoadOfficeFiles([FromForm] IFormFileCollection files)
    {
        
        return Json(await officeFiles.RegisterAsync(files));
    }
}