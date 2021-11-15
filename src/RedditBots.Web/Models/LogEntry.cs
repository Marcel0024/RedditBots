namespace RedditBots.Web.Models;

public class LogEntry
{
    public string LogName { get; set; }

    public string Message { get; set; }

    public string LogLevel { get; set; }

    public string LogDateTime { get; set; }

    public bool Notify { get; set; } = true;
}
