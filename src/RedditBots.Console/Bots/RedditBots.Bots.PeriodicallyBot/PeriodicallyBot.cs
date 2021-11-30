using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit;
using Reddit.Controllers.EventArgs;
using RedditBots.Libraries.BotFramework;
using RedditBots.Bots.PeriodicallyBot.Settings;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.Bots.PeriodicallyBot;

/// <summary>
/// Bot that runs once a day
/// </summary>
public class PeriodicallyBot : RedditBotBackgroundService
{
    protected override bool MonitorPosts => false;
    protected override bool MonitorComments => false;

    private readonly PeriodicallyBotSettings _periodicallyBotSettings;

    public PeriodicallyBot(
        ILogger<PeriodicallyBot> logger,
        IOptions<MonitorSettings> monitorSettings,
        IOptions<PeriodicallyBotSettings> periodicallyBotSettings) 
        : base(logger, monitorSettings)
    {
        _periodicallyBotSettings = periodicallyBotSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await HibernateUntilNextRun(stoppingToken);

            Logger.LogInformation($"I have awoken");

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

        Logger.LogInformation($"Sleeping for {timeUntilNextRun.Hours} hours and {timeUntilNextRun.Minutes} minutes");

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
        var posts = RedditClient.Subreddit("CSharp").Posts.Hot;
        var post = posts.FirstOrDefault(p => p.Author == "AutoModerator"
            && p.Title.Contains($"[{now:MMMM yyyy}]")
            && p.Title.Contains("your side projects!"));

        if (post == null)
        {
            Logger.LogWarning("Could not find Thread of monthly projects on /r/CSharp");
            return;
        }

        var replyText = new StringBuilder()
            .Append("A while ago i created some reddit bots that runs on a console application. It was nice for a while then i decided to host them on a Raspberry Pi.")
            .Append("\n\n")
            .Append("Now they auto deploy to the raspberry pi via Azure DevOps, and i have a web app that receives all the logs from the console and saves them to a CosmosDb and streams them to the browser via SignalR. Now if i only had a good idea for a bot...")
            .Append("\n\n")
            .Append($"The repo is here: https://github.com/Marcel0024/RedditBots and the live logs can be viewed here: https://RedditBots.azurewebsites.net")
            .Append("\n\n")
            .Append($"Everything in .NET 6 & Any feedback is welcome!")
            .ToString();

        await post.ReplyAsync(replyText);

        Logger.LogInformation($"Posted comment in r/{post.Subreddit} - {post.Title}");
    }

    protected override void C_NewCommentsUpdated(object sender, CommentsUpdateEventArgs e)
    { }

    protected override void C_NewPostsUpdated(object sender, PostsUpdateEventArgs e)
    { }
}
