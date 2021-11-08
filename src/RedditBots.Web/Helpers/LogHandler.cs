using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedditBots.Web.Hubs;
using RedditBots.Web.Models;

namespace RedditBots.Web.Helpers
{
    public class LogHandler
    {
        private const int _totalLogsHistory = 500;
        private readonly IHubContext<LogHub, ILogClient> _hubContext;

        public DateTime? LastLogDateTime { get; set; }

        public List<LogEntry> LastLogs { get; set; } = new List<LogEntry>(_totalLogsHistory);

        public LogHandler(IHubContext<LogHub, ILogClient> hubContext, IHostEnvironment env)
        {
            _hubContext = hubContext;

            if (env.IsDevelopment())
            {
                for (int i = 0; i < 10; i++)
                {
                    LastLogs.Add(new LogEntry
                    {
                        LogLevel = "Information",
                        LogName = $"abra.daba.doo{i}",
                        Message = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                        Notify = true
                    });
                }

                LastLogDateTime = DateTime.Now;
            }
        }

        internal async Task LogAsync(LogEntry entry)
        {
            LastLogDateTime = DateTime.Now;

            await _hubContext.Clients.All.Log(entry);
            await _hubContext.Clients.All.UpdateLastDateTime(LastLogDateTime.Value.ToShortTimeString());

            if (LastLogs.Count == _totalLogsHistory)
            {
                var lastLog = LastLogs.FirstOrDefault(ll => ll.LogLevel == LogLevel.Debug.ToString());

                if (lastLog != null)
                {
                    LastLogs.Remove(lastLog);
                }
            }

            if (Enum.Parse<LogLevel>(entry.LogLevel) >= LogLevel.Debug)
            {
                entry.Notify = false;
                LastLogs.Add(entry);
            }
        }
    }
}
