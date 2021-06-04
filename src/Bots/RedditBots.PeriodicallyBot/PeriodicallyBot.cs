﻿using Microsoft.Extensions.Hosting;
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

namespace RedditBots.PeriodicallyBot
{
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
            _botSetting = monitorSettings.Value.Settings.Find(ms => ms.BotName == nameof(PeriodicallyBot)) ?? throw new ArgumentNullException("No bot settings found");

            _redditClient = new RedditClient(_botSetting.AppId, _botSetting.RefreshToken, _botSetting.AppSecret);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Started {_botSetting.BotName} in {_env.EnvironmentName}");

            while (!stoppingToken.IsCancellationRequested)
            {
                await _hibernateUntilNextRun(stoppingToken);

                _logger.LogInformation($"I have awoken");

                _executeTasks();
            }
        }

        private async Task _hibernateUntilNextRun(CancellationToken stoppingToken)
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

        private void _executeTasks()
        {
            var now = DateTime.UtcNow;

            foreach (var task in _periodicallyBotSettings.PeriodicTasks)
            {
                if (task.DayOfTheMonth == now.Day
                    && task.TaskType == TaskType.PostToCSharpMonthlyThread)
                {
                    _postToCSharpMonthlyThread(now);
                }
            }
        }

        private void _postToCSharpMonthlyThread(DateTime now)
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
                .Append("I've been working on a Console Application that can run multiple reddit bots (now also with a discord bot) AND have the logs streamed to a web app (an angular app), which you can monitor live.")
                .Append("The bots themselves are not that interesting but building them has been fun. It currently has 3 bots running on it and it's hosted on a Raspberry Pi")
                .Append("\n\n")
                .Append("Each bot is a BackgroundService and with a custom ILogger i send all logs via http to a site, which streams it to a client with SignalR, of course everything in .NET 5")
                .Append("\n\n")
                .Append($"The Console is hosted in Docker and auto-deployed to the pi via Azure Pipelines")
                .Append("\n\n")
                .Append($"The logs website is deployed with the help of Bicep templates to Azure with Github Actions")
                .Append("\n\n")
                .Append($"The repo is here: https://github.com/Marcel0024/RedditBots and the live logs can be viewed here: https://redditbots.azurewebsites.net")
                .Append("\n\n")
                .Append($"Any feedback is welcome!")
                .ToString();

            post.Reply(replyText);

            _logger.LogInformation($"Posted comment in r/{post.Subreddit} - {post.Title}");
        }
    }
}
