using System.Collections.Concurrent;

namespace RedditBots.Libraries.Logging;

public class HttpLoggerQueue
{
    public readonly ConcurrentQueue<HttpLogEntry> Messages = new ConcurrentQueue<HttpLogEntry>();
}
