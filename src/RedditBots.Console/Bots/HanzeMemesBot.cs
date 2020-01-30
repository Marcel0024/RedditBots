using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using RedditBots.Settings;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.Bots
{
    /// <summary>
    /// FlairReminderBots replies to all new post with a 'Reminder' message to set a flair
    /// Than it checks if a flair has been added to new posts, if so it deletes its own 'Reminder' message
    /// </summary>
    public class HanzeMemesBot : BackgroundService
    {
        private readonly ILogger<HanzeMemesBot> _logger;
        private readonly IHostEnvironment _env;
        private readonly BotSetting _botSetting;
        private readonly RedditClient _redditClient;

        private readonly List<Subreddit> _monitoringSubreddits = new List<Subreddit>();

        public HanzeMemesBot(
            ILogger<HanzeMemesBot> logger,
            IHostEnvironment env,
            IOptions<MonitorSettings> monitorSettings)
        {
            _logger = logger;
            _env = env;
            _botSetting = monitorSettings.Value.Settings.Find(ms => ms.BotName == nameof(HanzeMemesBot)) ?? throw new ArgumentNullException("No bot settings found");

            _redditClient = new RedditClient(_botSetting.AppId, _botSetting.RefreshToken, _botSetting.AppSecret);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Started {_botSetting.BotName} in {_env.EnvironmentName}");

            _startMonitoringSubreddits();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var subreddit in _monitoringSubreddits)
                    {
                        _monitorPostsForAddedFlair(subreddit);
                    }
                }
                catch (Exception e) when (e.GetType().Name.StartsWith("Reddit"))
                {
                    _logger.LogWarning($"{DateTime.Now} Reddit threw {e.GetType().Name}");

                    Task.Delay(1000 * 60, stoppingToken); // wait a minute, reddit is probably down
                }

                Task.Delay(2000, stoppingToken);
            }

            return Task.CompletedTask;
        }

        private void _startMonitoringSubreddits()
        {
            foreach (var subredditToMonitor in _botSetting.Subreddits)
            {
                var subreddit = _redditClient.Subreddit(subredditToMonitor);

                subreddit.Posts.GetNew();
                subreddit.Posts.MonitorNew();

                subreddit.Posts.NewUpdated += C_NewPostsUpdated;

                _monitoringSubreddits.Add(subreddit);

                _logger.LogDebug($"Started monitoring {subredditToMonitor}");
            }
        }

        private void _monitorPostsForAddedFlair(Subreddit subreddit)
        {
            var newPosts = subreddit.Posts.New;

            foreach (var newPost in newPosts)
            {
                if (!string.IsNullOrWhiteSpace(newPost.Listing.LinkFlairText))
                {
                    _checkForReminderComment(newPost);
                }
            }
        }

        private void _checkForReminderComment(Post newPost)
        {
            var oldComments = newPost.Comments.Old;

            foreach (var oldComment in oldComments)
            {
                if (string.Equals(oldComment.Author, _botSetting.BotName, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation($"{DateTime.Now} flair detected, removing own comment in post of /u/{newPost.Author}");

                    oldComment.Delete();
                }
            }
        }

        private void C_NewPostsUpdated(object sender, PostsUpdateEventArgs e)
        {
            foreach (Post post in e.Added)
            {
                _logger.LogInformation($"{DateTime.Now} new post from /u/{post.Author} in /r/{post.Subreddit} leaving comment");

                post.Reply(string.Format(_botSetting.DefaultReplyMessage, post.Author) + _botSetting.MessageFooter);
            }
        }
    }
}
