using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RedditBots.Web.Helpers;
using RedditBots.Web.Hubs;
using RedditBots.Web.Models;
using System.Threading.Tasks;

namespace RedditBots.Web.Controllers
{
    [ApiController, Route("[controller]/[action]")]
    [ApiKeyRequired]
    public class ApiController : ControllerBase
    {
        private IHubContext<LogHub, ILogClient> _hubContext;

        public ApiController(IHubContext<LogHub, ILogClient> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> Log([FromBody] LogEntry entry)
        {
            await _hubContext.Clients.All.LogAsync(entry);

            return Ok();
        }
    }
}