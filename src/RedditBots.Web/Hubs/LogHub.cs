using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using RedditBots.Web.Helpers;
using System;
using System.Threading.Tasks;

namespace RedditBots.Web.Hubs
{
    public class LogHub : Hub<ILogClient>
    {
        private readonly LogHandler _logsHandler;
        private readonly IHostEnvironment _env;

        public static int TotalViewers { get; private set; }

        public LogHub(LogHandler logsHandler, IHostEnvironment env)
        {
            _logsHandler = logsHandler;
            _env = env;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            foreach (var log in _logsHandler.LastLogs)
            {
                await Clients.Caller.Log(log);
            }

            if (_env.IsDevelopment())
            {
                for (int i = 0; i < 10; i++)
                {
                    await Clients.Caller.Log(new Models.LogEntry
                    {
                        LogLevel = "Information",
                        LogName = $"abra.daba.doo{i}",
                        Message = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Notify = false
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
}
