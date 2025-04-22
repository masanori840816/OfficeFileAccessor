using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OfficeFileAccessor.AppUsers;
using OfficeFileAccessor.AppUsers.DTO;

namespace OfficeFileAccessor.Logs;

public class ClientLogController(ILogger<ClientLogController> Logger,
    IApplicationUserService Users): Controller
{
    [HttpGet("/api/logs/pageaccess")]
    public async Task<IActionResult> LogPageAccess([FromQuery] string page)
    {
        DisplayUser? user = await Users.GetSignedInUserAsync(User) ?? DisplayUser.AnonymousUser();
        Logger.LogInformation(JsonSerializer.Serialize(LogValue.LogPageOpen(user, page)));
        return Ok();
    }
}