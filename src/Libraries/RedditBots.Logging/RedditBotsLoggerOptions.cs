using Microsoft.Extensions.Logging;

namespace RedditBots.Logging
{
    public class RedditBotsLoggerOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;

        public string Url { get; set; }

        public string ApiKey { get; set; }
    }
}
