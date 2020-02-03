using Microsoft.Extensions.Logging;
using System;

namespace RedditBots.Libraries.Logging
{
    class HttpLogger : ILogger
    {
        private readonly string _name;
        private readonly HttpLoggerQueue _queue;
        private readonly HttpLoggerOptions _config;


        public HttpLogger(string name, HttpLoggerQueue queue, HttpLoggerOptions config)
        {
            _name = name;
            _queue = queue;
            _config = config;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _config.LogLevel;
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
            _queue.Messages.Enqueue(new HttpLogEntry
            {
                LogName = logName,
                LogLevel = logLevel.ToString(),
                Message = $"{DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss")} - {message}",
            });
        }
    }
}
