using RedditBots.Web.Models;

namespace RedditBots.Web.Hubs;

public interface ILogClient
{
    Task Log(LogEntry log);

    Task UpdateViewers(int viewers);
}
