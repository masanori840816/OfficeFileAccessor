using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OfficeFileAccessor.Apps;

public class AppController: Controller
{
    // To get XSRF token
    [AllowAnonymous]
    [HttpGet("/api/xsrf-token")]
    public IActionResult GetXSRFToken()
    {
        return Ok("XSRF token set");
    }
}
