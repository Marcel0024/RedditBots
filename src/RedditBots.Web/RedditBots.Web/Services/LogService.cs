using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RedditBots.Web.Data;
using RedditBots.Web.Data.Enums;
using RedditBots.Web.Hubs;
using RedditBots.Web.Models;
using System.Globalization;

namespace RedditBots.Web.Helpers;

public class LogService
{
    private const int _totalLogsHistory = 500;
    private readonly IHubContext<LogHub, ILogClient> _hubContext;
    private readonly LogsDbContext _dbContext;

    public LogService(
        LogsDbContext dbContext,
        IHubContext<LogHub,
        ILogClient> hubContext)
    {
        _hubContext = hubContext;
        _dbContext = dbContext;
    }

    internal async Task LogAsync(LogEntry entry)
    {
        _dbContext.Logs.Add(new Data.Models.Log
        {
            Level = Enum.Parse<LogLevel>(entry.LogLevel),
            Application = entry.LogName,
            Message = entry.Message,
            DateTime = DateTime.ParseExact(entry.LogDateTime, "o", CultureInfo.InvariantCulture)
        });

        await _dbContext.SaveChangesAsync();

        await _hubContext.Clients.All.Log(entry);
    }

    internal async Task<IEnumerable<LogEntry>> GetLastLogsAsync()
    {
        return await _dbContext.Logs
             .OrderByDescending(l => l.DateTime)
             .Take(_totalLogsHistory)
             .Select(l => new LogEntry
             {
                 LogDateTime = l.DateTime.ToString("o", CultureInfo.InvariantCulture),
                 LogLevel = l.Level.ToString(),
                 LogName = l.Application,
                 Message = l.Message,
                 Notify = false,
             })
             .ToArrayAsync();
    }
}
