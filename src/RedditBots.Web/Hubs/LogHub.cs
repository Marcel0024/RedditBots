using Microsoft.AspNetCore.SignalR;
using RedditBots.Web.Helpers;

namespace RedditBots.Web.Hubs;

public class LogHub : Hub<ILogClient>
{
    private readonly LogHandler _logsHandler;

    public static int TotalViewers { get; private set; }

    public LogHub(LogHandler logsHandler)
    {
        _logsHandler = logsHandler;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        foreach (var log in _logsHandler.LastLogs)
        {
            await Clients.Caller.Log(log);
        }

        await Clients.Caller.UpdateLastDateTime(_logsHandler.LastLogDateTime?.ToShortTimeString() ?? "");
        await Clients.All.UpdateViewers(++TotalViewers);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await Clients.All.UpdateViewers(--TotalViewers);

        await base.OnDisconnectedAsync(exception);
    }
}
