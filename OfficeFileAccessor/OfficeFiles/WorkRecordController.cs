using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeFileAccessor.Files.DTO;

namespace OfficeFileAccessor.OfficeFiles;

[AutoValidateAntiforgeryToken]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class WorkRecordController(IWorkRecordService WorkRecords): Controller
{
    [HttpGet("/api/records/download")]
    public async Task<IActionResult> DownloadRecords([FromQuery] long recordId)
    {
        DownloadFile result = await WorkRecords.DonwloadFileAsync(recordId);
        Response.Headers.Append("File-Name", WebUtility.UrlEncode(result.FileName));
        return File(result.FileData, result.MimeType, result.FileName);
    }
}