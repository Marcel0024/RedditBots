using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using RedditBots.Console.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.Console.Bots
{
    public class CheerfulBot : BackgroundService
    {
        private readonly ILogger<CheerfulBot> _logger;
        private readonly IHostEnvironment _env;
        private readonly BotSetting _botSetting;
        private readonly RedditClient _redditClient;

        private readonly List<Subreddit> _monitoringSubreddits = new List<Subreddit>();
        private readonly Random _random = new Random(69);

        private readonly string[] _quotes = new string[] {
            "Don’t give up when dark times come. The more storms you face in life, the stronger you’ll be. Hold on. Your greater is coming.",
            "The sky is everywhere, it begins at your feet.",
            "Stop feeling sorry for yourself and you will be happy.",
            "I’ve got nothing to do today but smile.",
            "Nobody is superior, nobody is inferior, but nobody is equal either. People are simply unique, incomparable. You are you, I am I.",
            "Let us be of good cheer, however, remembering that the misfortunes hardest to bear are those which never come.",
            "Laugh now, cry later.",
            "Cheer up, my dear. After every storm comes the sun. Happiness is waiting for you ahead.",
            "Once they move, they’re gone. Once you move, life starts over again.",
            "It’s better to have loved and lost than never to have loved at all.",
            "What doesn’t kill you makes you stronger.",
            "My entire life can be described in one sentence: It didn’t go as planned, and that’s okay." 
        };

        public CheerfulBot(
            ILogger<CheerfulBot> logger,
            IHostEnvironment env,
            IOptions<MonitorSettings> monitorSettings)
        {
            _logger = logger;
            _env = env;
            _botSetting = monitorSettings.Value.Settings.Find(ms => ms.BotName == nameof(CheerfulBot)) ?? throw new ArgumentNullException("No bot settings found");

            _redditClient = new RedditClient(_botSetting.AppId, _botSetting.RefreshToken, _botSetting.AppSecret);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Started {_botSetting.BotName} in {_env.EnvironmentName}");

            _startMonitoringSubreddits();

            return Task.CompletedTask;
        }

        private void _startMonitoringSubreddits()
        {
            foreach (var subredditToMonitor in _botSetting.Subreddits)
            {
                var subreddit = _redditClient.Subreddit(subredditToMonitor);

                subreddit.Comments.GetNew();
                subreddit.Comments.MonitorNew();
                subreddit.Comments.NewUpdated += C_NewCommentsUpdated;

                _monitoringSubreddits.Add(subreddit);

                _logger.LogDebug($"Started monitoring {subredditToMonitor}");
            }
        }

        private void C_NewCommentsUpdated(object sender, CommentsUpdateEventArgs e)
        {
            foreach (Comment comment in e.Added)
            {
                _logger.LogDebug($"{DateTime.Now} New comment detected of /u/{comment.Author} in /r/{comment.Subreddit}");

                if (comment.Body.StartsWith("I'm sad that", StringComparison.OrdinalIgnoreCase))
                {
                    _buildReplyComment(comment);
                }
            }
        }

        private void _buildReplyComment(Comment comment)
        {
            _logger.LogInformation($"{DateTime.Now} Sad user ({comment.Author}) detected in /r/{comment.Subreddit}, leaving cheerful comment");

            StringBuilder builder = new StringBuilder();

            builder
                .Append("> I'm sad that ...")
                .Append("\n\n")
                .Append(string.Format(_botSetting.DefaultReplyMessage, comment.Author))
                .Append("\n\n")
                .Append($"> *{_getRandomQuote()}*")
                .Append("\n\n")
                .Append(_botSetting.MessageFooter);

            comment.Reply(builder.ToString());
        }

        private string _getRandomQuote()
        {
            return _quotes[_random.Next(_quotes.Length - 1)];
        }
    }
}
