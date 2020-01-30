using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.Libraries.Logging
{
    public class HttpLoggerProcessor : BackgroundService
    {
        private readonly HttpLoggerQueue _queue;
        private readonly HttpLoggerService _service;

        public HttpLoggerProcessor(HttpLoggerQueue queue, HttpLoggerService service)
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
                    if (_queue.Messages.TryDequeue(out HttpLogEntry message))
                    {
                        await _service.PostLogAsync(JsonSerializer.Serialize(message), stoppingToken);
                    }
                }
                catch { }
            }
        }
    }
}
