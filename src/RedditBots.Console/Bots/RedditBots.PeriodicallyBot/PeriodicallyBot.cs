using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit;
using RedditBots.Framework;
using RedditBots.PeriodicallyBot.Settings;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackgroundService = RedditBots.Framework.BackgroundService;

namespace RedditBots.PeriodicallyBot;

/// <summary>
/// Bot that runs once a day
/// </summary>
public class PeriodicallyBot : BackgroundService
{
    private readonly ILogger<PeriodicallyBot> _logger;
    private readonly IHostEnvironment _env;
    private readonly PeriodicallyBotSettings _periodicallyBotSettings;
    private readonly BotSetting _botSetting;
    private readonly RedditClient _redditClient;

    public PeriodicallyBot(
        ILogger<PeriodicallyBot> logger,
        IHostEnvironment env,
        IOptions<MonitorSettings> monitorSettings,
        IOptions<PeriodicallyBotSettings> periodicallyBotSettings)
    {
        _logger = logger;
        _env = env;
        _periodicallyBotSettings = periodicallyBotSettings.Value;
        _botSetting = monitorSettings.Value.Settings.Find(ms => ms.BotName == nameof(PeriodicallyBot))
            ?? throw new ArgumentNullException("No bot settings found");

        _redditClient = new RedditClient(_botSetting.AppId, _botSetting.RefreshToken, _botSetting.AppSecret);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Started {_botSetting.BotName} in {_env.EnvironmentName}");

        while (!stoppingToken.IsCancellationRequested)
        {
            await HibernateUntilNextRun(stoppingToken);

            _logger.LogInformation($"I have awoken");

            await ExecuteTasksAsync();
        }
    }

    private async Task HibernateUntilNextRun(CancellationToken stoppingToken)
    {
        var now = DateTime.UtcNow;
        var tomorrow = now.AddDays(1);
        var nextRun = tomorrow.Date.Add(_periodicallyBotSettings.TimeOfExecution);

        if (nextRun.Subtract(now) >= TimeSpan.FromHours(24)) // Next run is today
        {
            nextRun = now.Date.Add(_periodicallyBotSettings.TimeOfExecution);
        }

        var timeUntilNextRun = nextRun.Subtract(now);

        _logger.LogInformation($"Sleeping for {timeUntilNextRun.Hours} hours and {timeUntilNextRun.Minutes} minutes");

        await Task.Delay(timeUntilNextRun, stoppingToken);
    }

    private async Task ExecuteTasksAsync()
    {
        var now = DateTime.UtcNow;

        foreach (var task in _periodicallyBotSettings.PeriodicTasks)
        {
            if (task.DayOfTheMonth == now.Day
                && task.TaskType == TaskType.PostToCSharpMonthlyThread)
            {
                await PostToCSharpMonthlyThreadAsync(now);
            }
        }
    }

    private async Task PostToCSharpMonthlyThreadAsync(DateTime now)
    {
        var posts = _redditClient.Subreddit("CSharp").Posts.Hot;
        var post = posts.FirstOrDefault(p => p.Author == "AutoModerator"
            && p.Title.Contains($"[{now:MMMM yyyy}]")
            && p.Title.Contains("your side projects!"));

        if (post == null)
        {
            _logger.LogWarning("Could not find Thread of monthly projects on /r/CSharp");
            return;
        }

        var replyText = new StringBuilder()
            .Append("A while ago i created some reddit bots that runs on a console application. It was nice for a while then i decided to host them on a Raspberry Pi.")
            .Append("\n\n")
            .Append("Now they auto deploy to the raspberry pi, and i have a web app that receives all the logs from the console and saves them to a CosmosDb and streams them to the browser via SignalR. Now i only need a good idea for a bot...")
            .Append("\n\n")
            .Append($"The repo is here: https://github.com/Marcel0024/RedditBots and the live logs can be viewed here: https://RedditBots.azurewebsites.net")
            .Append("\n\n")
            .Append($"Everything in .NET 6 & Any feedback is welcome!")
            .ToString();

        await post.ReplyAsync(replyText);

        _logger.LogInformation($"Posted comment in r/{post.Subreddit} - {post.Title}");
    }
}
