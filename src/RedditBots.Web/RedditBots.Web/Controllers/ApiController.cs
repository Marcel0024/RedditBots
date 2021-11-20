using Microsoft.AspNetCore.Mvc;
using RedditBots.Web.Helpers;
using RedditBots.Web.Models;

namespace RedditBots.Web.Controllers;

[ApiController, Route("[controller]/[action]")]
public class ApiController : ControllerBase
{
    private readonly LogService _logService;

    public ApiController(LogService logsHelper)
    {
        _logService = logsHelper;
    }

    [HttpPost, ApiKeyRequired]
    public async Task<IActionResult> Log([FromBody] LogEntry entry)
    {
        await _logService.LogAsync(entry);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetLastLogs()
    {
        return new JsonResult(new
        {
            logs = await _logService.GetLastLogsAsync()
        });
    }
}