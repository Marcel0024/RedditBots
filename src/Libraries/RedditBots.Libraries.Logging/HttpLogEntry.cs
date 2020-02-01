using Microsoft.Extensions.Logging;

namespace RedditBots.Libraries.Logging
{
    public class HttpLogEntry
    {
        public string Message { get; set; }

        public LogLevel LogLevel { get; set; }

        public string LogName { get; set; }
    }
}
