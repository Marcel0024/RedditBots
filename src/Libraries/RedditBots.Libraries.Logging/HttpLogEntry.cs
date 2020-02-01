using Microsoft.Extensions.Logging;

namespace RedditBots.Libraries.Logging
{
    public class HttpLogEntry
    {
        public string Message { get; set; }

        public string LogLevel { get; set; }

        public string LogName { get; set; }
    }
}
