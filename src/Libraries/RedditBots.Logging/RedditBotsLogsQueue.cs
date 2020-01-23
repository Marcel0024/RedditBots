using System.Collections.Concurrent;

namespace RedditBots.Logging
{
    public class RedditBotsLogsQueue
    {
        public readonly ConcurrentQueue<RedditBotsLogEntry> Messages = new ConcurrentQueue<RedditBotsLogEntry>();
    }
}
