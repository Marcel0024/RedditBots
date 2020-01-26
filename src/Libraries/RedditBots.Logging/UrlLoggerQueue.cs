using System.Collections.Concurrent;

namespace RedditBots.Logging
{
    public class UrlLoggerQueue
    {
        public readonly ConcurrentQueue<UrlLogEntry> Messages = new ConcurrentQueue<UrlLogEntry>();
    }
}
