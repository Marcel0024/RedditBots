using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace RedditBots.Logging
{
    class UrlLoggerProvider : ILoggerProvider
    {
        private readonly UrlLoggerOptions _config;
        private readonly UrlLoggerQueue _queue;
        private readonly ConcurrentDictionary<string, UrlLogger> _loggers = new ConcurrentDictionary<string, UrlLogger>();

        public UrlLoggerProvider(IOptions<UrlLoggerOptions> config, UrlLoggerQueue queue)
        {
            _config = config.Value;
            _queue = queue;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new UrlLogger(name, _queue, _config));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
