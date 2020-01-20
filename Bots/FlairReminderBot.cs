using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedditBots.Settings;
using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace RedditBots.Bots
{ 
    /// <summary>
    /// FlairReminderBots replies to all new post with a 'Reminder' message to set a flair
    /// Than it checks if a flair has been added to new posts, if so it deletes its own 'Reminder' message
    /// </summary>
    public class FlairReminderBot : BackgroundService
    {
        private readonly ILogger<FlairReminderBot> _logger;
        private readonly IHostEnvironment _env;
        private readonly MonitorSetting _monitorSettings;
        private readonly RedditClient _redditClient;

        public FlairReminderBot(
            ILogger<FlairReminderBot> logger,
            IHostEnvironment env,
            IOptions<MonitorSettings> monitorSettings)
        {
            _logger = logger;
            _env = env;
            _monitorSettings = monitorSettings.Value.Settings.Find(ms => ms.Bot == nameof(FlairReminderBot)) ?? throw new ArgumentNullException("No bot settings found");

            _redditClient = new RedditClient(_monitorSettings.AppId, _monitorSettings.RefreshToken, _monitorSettings.AppSecret);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Started {_monitorSettings.BotName} in {_env.EnvironmentName}");

            var subreddit = _redditClient.Subreddit(_monitorSettings.Subreddits.First());

            subreddit.Posts.GetNew();
            subreddit.Posts.MonitorNew();

            subreddit.Posts.NewUpdated += C_NewPostsUpdated;

            while (!stoppingToken.IsCancellationRequested)
            {
                var newPosts = subreddit.Posts.New;

                foreach (var newPost in newPosts)
                {
                    if (!string.IsNullOrWhiteSpace(newPost.Listing.LinkFlairText))
                    {
                        var oldComments = newPost.Comments.Old;

                        foreach (var oldComment in oldComments)
                        {
                            if (string.Equals(oldComment.Author, _monitorSettings.BotName, StringComparison.OrdinalIgnoreCase))
                            {
                                _logger.LogInformation($"{DateTime.Now} flair detected removing own comment in post of {newPost.Author}");

                                oldComment.Delete();
                            }
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        private void C_NewPostsUpdated(object sender, PostsUpdateEventArgs e)
        {
            foreach (Post post in e.Added)
            {
                _logger.LogInformation($"{DateTime.Now} new post from {post.Author} in /r/{post.Subreddit} leaving comment");

                post.Reply(string.Format(_monitorSettings.DefaultReplyMessage, post.Author) + _monitorSettings.MessageFooter);
            }
        }
    }
}
