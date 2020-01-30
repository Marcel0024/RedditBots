using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.Libraries.Logging
{
    public class HttpLoggerProcessor : BackgroundService
    {
        private readonly HttpLoggerQueue _queue;
        private readonly HttpLoggerService _service;
        private readonly ILogger<HttpLoggerProcessor> _logger;
        private readonly int _maxQueue = 10000;

        public HttpLoggerProcessor(HttpLoggerQueue queue, HttpLoggerService service, ILogger<HttpLoggerProcessor> logger)
        {
            _queue = queue;
            _service = service;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield(); // https://github.com/dotnet/extensions/issues/2149

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_queue.Messages.Count > 10000)
                    {
                        _queue.Messages.Clear(); // Panic
                        _logger.LogWarning($"Dumped queue of {_maxQueue}");
                    }

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
