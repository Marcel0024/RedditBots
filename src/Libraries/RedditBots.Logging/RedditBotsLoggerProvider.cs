using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace RedditBots.Logging
{
    class RedditBotsLoggerProvider : ILoggerProvider
    {
        private readonly RedditBotsLoggerOptions _config;
        private readonly RedditBotsLogsQueue _queue;
        private readonly ConcurrentDictionary<string, RedditBotsLogger> _loggers = new ConcurrentDictionary<string, RedditBotsLogger>();

        public RedditBotsLoggerProvider(IOptions<RedditBotsLoggerOptions> config, RedditBotsLogsQueue queue)
        {
            _config = config.Value;
            _queue = queue;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new RedditBotsLogger(name, _queue, _config));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
