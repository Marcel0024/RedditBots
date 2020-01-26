using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.Logging
{
    public class UrlLoggerProcessor : BackgroundService
    {
        private readonly UrlLoggerQueue _queue;
        private readonly UrlLoggerService _service;

        public UrlLoggerProcessor(UrlLoggerQueue queue, UrlLoggerService service)
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
                    if (_queue.Messages.TryDequeue(out UrlLogEntry message))
                    {
                        await _service.PostLogAsync(JsonSerializer.Serialize(message));
                    }
                }
                catch { }
            }
        }
    }
}
