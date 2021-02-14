using RedditBots.Web2.Models;
using System.Threading.Tasks;

namespace RedditBots.Web2.Hubs
{
    public interface ILogClient
    {
        Task Log(LogEntry log);

        Task UpdateLastDateTime(string time);

        Task UpdateViewers(int viewers);
    }
}
