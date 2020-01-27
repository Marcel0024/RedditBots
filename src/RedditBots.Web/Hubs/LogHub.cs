using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace RedditBots.Web.Hubs
{
    public class LogHub : Hub<ILogClient>
    {
        public static int TotalViewers { get; private set; }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            await Clients.All.UpdateViewers(++TotalViewers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.UpdateViewers(--TotalViewers);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
