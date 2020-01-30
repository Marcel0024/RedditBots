using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace RedditBots.Libraries.Logging
{
    [ProviderAlias("Http")]
    class HttpLoggerProvider : ILoggerProvider
    {
        private readonly HttpLoggerOptions _config;
        private readonly HttpLoggerQueue _queue;
        private readonly ConcurrentDictionary<string, HttpLogger> _loggers = new ConcurrentDictionary<string, HttpLogger>();

        public HttpLoggerProvider(IOptions<HttpLoggerOptions> config, HttpLoggerQueue queue)
        {
            _config = config.Value;
            _queue = queue;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new HttpLogger(name, _queue, _config));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
