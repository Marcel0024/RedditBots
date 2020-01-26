using Microsoft.Extensions.Logging;
using System;

namespace RedditBots.Logging
{
    class UrlLogger : ILogger
    {
        private readonly string _name;
        private readonly UrlLoggerQueue _queue;
        private readonly UrlLoggerOptions _config;


        public UrlLogger(string name, UrlLoggerQueue queue, UrlLoggerOptions config)
        {
            _name = name;
            _queue = queue;
            _config = config;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == _config.LogLevel
                && !string.IsNullOrWhiteSpace(_config.Url);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                LogMessage(logLevel, _name, eventId.Id, message, exception);
            }
        }

        public virtual void LogMessage(LogLevel logLevel, string logName, int eventId, string message, Exception exception)
        {
            // Queue log message
            _queue.Messages.Enqueue(new UrlLogEntry
            {
                LogName = logName,
                LogLevel = logLevel.ToString(),
                Message = message,
            });
        }
    }
}
