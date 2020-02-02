using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Wrap;
using System;
using System.Net.Http;
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
        private readonly AsyncPolicyWrap _policy;

        private readonly int _maxQueue = 50000;

        public HttpLoggerProcessor(HttpLoggerQueue queue, HttpLoggerService service, ILogger<HttpLoggerProcessor> logger)
        {
            _queue = queue;
            _service = service;
            _logger = logger;

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<OperationCanceledException>()
                .WaitAndRetryAsync(new[] { TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(3) });

            var circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<OperationCanceledException>()
                .CircuitBreakerAsync(exceptionsAllowedBeforeBreaking: 3, durationOfBreak: TimeSpan.FromMinutes(3));

            _policy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield(); // https://github.com/dotnet/extensions/issues/2149

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queue.Messages.Count > _maxQueue)
                {
                    _queue.Messages.Clear();
                    _logger.LogWarning($"Dumped {_maxQueue} logs in queue.");
                }

                if (_queue.Messages.TryDequeue(out HttpLogEntry message))
                {
                    try
                    {
                        await _policy.ExecuteAsync(async () => await _service.PostLogAsync(JsonSerializer.Serialize(message), stoppingToken));
                    }
                    catch (Exception e) when (
                               e is HttpRequestException
                            || e is OperationCanceledException
                            || e is BrokenCircuitException)
                    {
                        if (Enum.Parse<LogLevel>(message.LogLevel) > LogLevel.Debug)
                        {
                            _queue.Messages.Enqueue(message);
                        }
                    }
                }
            }
        }
    }
}
