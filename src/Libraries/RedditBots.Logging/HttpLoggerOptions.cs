using Microsoft.Extensions.Logging;

namespace RedditBots.Libraries.Logging
{
    public class HttpLoggerOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;

        public string Url { get; set; }

        public string ApiKey { get; set; }
    }
}
