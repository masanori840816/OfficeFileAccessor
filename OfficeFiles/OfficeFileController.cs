using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeFileAccessor.AppUsers;
using OfficeFileAccessor.AppUsers.DTO;
using OfficeFileAccessor.Files.DTO;

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
        DownloadFile file = await officeFiles.RegisterAsync(files);
        return File(file.FileData, file.MimeType, file.FileName);
    }
}