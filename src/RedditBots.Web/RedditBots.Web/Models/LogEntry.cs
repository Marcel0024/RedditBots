using MessagePack;

namespace RedditBots.Web.Models;

public class LogEntry
{
    public string LogName { get; init; }

    public string Message { get; init; }

    public string LogLevel { get; init; }

    public string LogDateTime { get; init; }

    public bool Notify { get; init; } = true;
}
