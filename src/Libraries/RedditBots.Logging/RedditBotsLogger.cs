using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedditBots.Logging
{
    class RedditBotsLogger : ILogger
    {
        private readonly string _name;
        private readonly RedditBotsLogsQueue _queue;
        private readonly RedditBotsLoggerOptions _config;


        public RedditBotsLogger(string name, RedditBotsLogsQueue queue, RedditBotsLoggerOptions config)
        {
            _name = name;
            _queue = queue;
            _config = config;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == _config.LogLevel;
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
            if (logName == "System.Net.Http.HttpClient.RedditBotsLoggerService.LogicalHandler")
            {
                return;
            }

            // Queue log message
            _queue.Messages.Enqueue(new RedditBotsLogEntry
            {
                LogLevel = logLevel.ToString(),
                Message = message,
            });
        }
    }
}
