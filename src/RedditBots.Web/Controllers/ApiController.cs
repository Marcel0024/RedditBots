using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RedditBots.Web.Helpers;
using RedditBots.Web.Hubs;
using RedditBots.Web.Models;
using System;
using System.Threading.Tasks;

namespace RedditBots.Web.Controllers
{
    [ApiController, Route("[controller]/[action]")]
    [ApiKeyRequired]
    public class ApiController : ControllerBase
    {
        private IHubContext<LogHub, ILogClient> _hubContext;
        private readonly LogsHelper _logsHelper;

        public ApiController(
            IHubContext<LogHub, ILogClient> hubContext,
            LogsHelper logsHelper)
        {
            _hubContext = hubContext;
            _logsHelper = logsHelper;
        }

        [HttpPost]
        public async Task<IActionResult> Log([FromBody] LogEntry entry)
        {
            _logsHelper.LastLogDateTime = DateTime.Now;

            await _hubContext.Clients.All.Log(entry);
            await _hubContext.Clients.All.UpdateLastDateTime(_logsHelper.LastLogDateTime.Value.ToShortTimeString());

            return Ok();
        }
    }
}