using Microsoft.AspNetCore.SignalR;

namespace RedditBots.Web.Hubs
{
    public class LogHub : Hub<ILogClient>
    { }
}
