using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.Libraries.BotFramework;

/// <summary>
/// https://github.com/dotnet/runtime/issues/36063
/// </summary>
public abstract class BackgroundService : IHostedService, IDisposable
{
    protected readonly BotSetting BotSetting;
    protected readonly ILogger<BackgroundService> Logger;
    private readonly CancellationTokenSource _stoppingCts = new();

    public BackgroundService(ILogger<BackgroundService> logger, IOptions<MonitorSettings> monitorSettings)
    {
        BotSetting = monitorSettings.Value.Settings.Single(ms => ms.BotName == GetType().Name);
        Logger = logger;
    }

    public Task ExecutingTask { get; private set; }

    protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        if (BotSetting.IsEnabled)
        {
            ExecutingTask = Task.Run(() => ExecuteAsync(_stoppingCts.Token), cancellationToken).ContinueWith((t) =>
            {
                if (t.IsFaulted)
                {
                    Logger.LogCritical($"Crashed. {t.Exception.Message}");
                }
            }, cancellationToken);
        }
        else
        {
            Logger.LogInformation("Not starting bot. Bot is not enabled.");
        }

        return Task.CompletedTask;
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        if (ExecutingTask == null)
        {
            return;
        }

        try
        {
            _stoppingCts.Cancel();
        }
        finally
        {
            await Task.WhenAny(ExecutingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }

    public virtual void Dispose()
    {
        _stoppingCts.Cancel();
    }
}