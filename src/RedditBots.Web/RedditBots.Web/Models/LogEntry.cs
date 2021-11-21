using MessagePack;
using System.Globalization;

namespace RedditBots.Web.Models;

public class LogEntry
{
    public string LogName { get; init; }

    public string Message { get; init; }

    public string LogLevel { get; init; }

    public string LogDateTime { get; init; } = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);

    public bool Notify { get; init; } = true;
}
