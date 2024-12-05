using Microsoft.AspNetCore.Mvc;

namespace OfficeFileAccessor.OfficeFiles;
public class OfficeFileController(ILogger<OfficeFileController> logger): Controller
{
    [HttpGet("/api/files")]
    public string GetFileNames()
    {
        logger.LogInformation("GetFileNames");
        return "Hello World!";
    }
    [HttpPost("/api/files")]
    public IActionResult LoadOfficeFiles([FromForm] IFormFileCollection files)
    {
        logger.LogInformation($"Files? {files?.Count}");
        return Ok("Hello");
    }
}