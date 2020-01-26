using RedditBots.Web.Models;
using System.Threading.Tasks;

namespace RedditBots.Web.Hubs
{
    public interface ILogClient
    {
        Task LogAsync(LogEntry log);
    }
}
