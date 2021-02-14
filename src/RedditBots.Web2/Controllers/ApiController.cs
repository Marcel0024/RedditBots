using Microsoft.AspNetCore.Mvc;
using RedditBots.Web2.Helpers;
using RedditBots.Web2.Models;
using System.Threading.Tasks;

namespace RedditBots.Web2.Controllers
{
    [ApiController, Route("[controller]/[action]")]
    [ApiKeyRequired]
    public class ApiController : ControllerBase
    {
        private readonly LogHandler _logHandler;

        public ApiController(LogHandler logsHelper)
        {
            _logHandler = logsHelper;
        }

        [HttpPost]
        public async Task<IActionResult> Log([FromBody] LogEntry entry)
        {
            await _logHandler.LogAsync(entry);

            return Ok();
        }
    }
}