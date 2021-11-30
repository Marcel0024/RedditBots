using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.Libraries.BotFramework
{
    /// <summary>
    /// Initilizes RedditClient and starts monitoring subreddits in bot settings.
    /// With the abstract callbacks you can implement what the bot does with new comments/posts
    /// </summary>
    public abstract class RedditBotBackgroundService : BackgroundService
    {
        protected readonly RedditClient RedditClient;
        protected readonly List<Subreddit> MonitoringSubreddits = new();

        protected abstract bool MonitorPosts { get; }
        protected abstract bool MonitorComments { get; }

        public RedditBotBackgroundService(ILogger<RedditBotBackgroundService> logger, IOptions<MonitorSettings> monitorSettings)
            : base(logger, monitorSettings)
        {
            RedditClient = BotSetting.IsEnabled
                ? new RedditClient(BotSetting.AppId, BotSetting.RefreshToken, BotSetting.AppSecret)
                : null;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            StartMonitoringSubreddits();
            return Task.CompletedTask;
        }

        private void StartMonitoringSubreddits()
        {
            foreach (var subredditToMonitor in BotSetting.Subreddits)
            {
                var subreddit = RedditClient.Subreddit(subredditToMonitor);

                if (MonitorPosts)
                {
                    subreddit.Posts.GetNew();
                    subreddit.Posts.MonitorNew();
                    subreddit.Posts.NewUpdated += C_NewPostsUpdated;
                }

                if (MonitorComments)
                {
                    subreddit.Comments.GetNew();
                    subreddit.Comments.MonitorNew();
                    subreddit.Comments.NewUpdated += C_NewCommentsUpdated;
                }

                MonitoringSubreddits.Add(subreddit);

                Logger.LogDebug($"Started monitoring {subredditToMonitor}");
            }
        }

        protected abstract void C_NewCommentsUpdated(object sender, CommentsUpdateEventArgs e);

        protected abstract void C_NewPostsUpdated(object sender, PostsUpdateEventArgs e);
    }
}
