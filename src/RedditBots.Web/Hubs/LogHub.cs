using Microsoft.AspNetCore.SignalR;
using RedditBots.Web.Helpers;
using System;
using System.Threading.Tasks;

namespace RedditBots.Web.Hubs
{
    public class LogHub : Hub<ILogClient>
    {
        private readonly LogsHelper _logsHelpder;
        public static int TotalViewers { get; private set; }

        public LogHub(LogsHelper logsHelper)
        {
            _logsHelpder = logsHelper;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            foreach (var log in _logsHelpder.LastLogs)
            {
                await Clients.Caller.Log(log);
            }

            await Clients.All.UpdateViewers(++TotalViewers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.UpdateViewers(--TotalViewers);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
