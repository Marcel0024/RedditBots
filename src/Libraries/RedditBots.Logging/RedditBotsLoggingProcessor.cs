using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.Logging
{
    public class RedditBotsLoggingProcessor : BackgroundService
    {
        private readonly RedditBotsLogsQueue _queue;
        private readonly RedditBotsLoggerService _service;

        public RedditBotsLoggingProcessor(RedditBotsLogsQueue queue, RedditBotsLoggerService service)
        {
            _queue = queue;
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield(); // https://github.com/dotnet/extensions/issues/2149

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _queue.Messages.TryDequeue(out RedditBotsLogEntry message);

                    await SendLogAsync(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"FAILED {e.GetType().Name}");
                }
            }
        }

        private Task SendLogAsync(RedditBotsLogEntry message)
        {
            return _service.PostLogAsync(JsonSerializer.Serialize(message));
        }
    }
}
