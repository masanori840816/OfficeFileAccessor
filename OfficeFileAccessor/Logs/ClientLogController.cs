using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeFileAccessor.AppUsers;
using OfficeFileAccessor.AppUsers.DTO;

namespace OfficeFileAccessor.Logs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ClientLogController: Controller
{
    private static readonly NLog.Logger accessLogger = NLog.LogManager.GetLogger("AccessLogger");
    private readonly IApplicationUserService users;

    public ClientLogController(IApplicationUserService users)
    {
        this.users = users;
    }
    [HttpGet("/api/logs/pageaccess")]
    public async Task<IActionResult> LogPageAccess([FromQuery] string page)
    {
        DisplayUser? user = await users.GetSignedInUserAsync(User) ?? DisplayUser.AnonymousUser();
        accessLogger.Info(JsonSerializer.Serialize(LogValue.LogPageOpen(user, page)));
        return Ok();
    }
}