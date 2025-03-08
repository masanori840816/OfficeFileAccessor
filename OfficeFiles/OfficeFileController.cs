using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeFileAccessor.AppUsers;
using OfficeFileAccessor.AppUsers.DTO;
using OfficeFileAccessor.Files.DTO;
using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles;

[AutoValidateAntiforgeryToken]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OfficeFileController(ILogger<OfficeFileController> Logger, IOfficeFileService OfficeFiles,
    IApplicationUserService Users): Controller
{
    [HttpGet("/api/files")]
    public string GetFileNames()
    {
        Logger.LogInformation("GetFileNames");
        return "Viva Las Vegas!";
    }
    [HttpPost("/api/files")]
    public async Task<IActionResult> LoadOfficeFiles([FromForm] IFormFileCollection files)
    {
        DisplayUser? user = await Users.GetSignedInUserAsync(User);
        if(user == null)
        {
            return Unauthorized();
        }
        DownloadFile file = await OfficeFiles.RegisterAsync(files, user);
        return File(file.FileData, file.MimeType, file.FileName);
    }
    [HttpGet("/api/files/previewsheets")]
    public async Task<IActionResult> GetPreviewSheets([FromQuery] long fileid)
    {
        return Json(await OfficeFiles.GetPreviewSheetsAsync(fileid));
    }
    [HttpGet("/api/files/sheets")]
    public async Task<IActionResult> GetOfficeFileSheet([FromQuery] long? sheetid)
    {
        OfficeFile? result = await OfficeFiles.GetOfficeFileSheetAsync(sheetid);
        if(result == null)
        {
            return BadRequest("The Specified office file was not found");
        }
        return Json(result);
    }
}