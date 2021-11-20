using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using RedditBots.Web.Data.Enums;
using RedditBots.Web.Models;
using System.Globalization;

namespace RedditBots.Web.Hubs;

public class LogHub : Hub<ILogClient>
{
    private readonly IHostEnvironment _env;

    public static int TotalViewers { get; private set; }

    public LogHub(IHostEnvironment env)
    {
        _env = env;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        if (_env.IsDevelopment())
        {
            for (int i = 0; i < 2; i++)
            {
                await Clients.All.Log(new LogEntry
                {
                    LogName = "Dev TEST",
                    LogLevel = LogLevel.Information.ToString(),
                    Message = $"Working {i}..",
                    LogDateTime = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                    Notify = true
                });
            }
        }

        await Clients.All.UpdateViewers(++TotalViewers);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await Clients.All.UpdateViewers(--TotalViewers);

        await base.OnDisconnectedAsync(exception);
    }
}
