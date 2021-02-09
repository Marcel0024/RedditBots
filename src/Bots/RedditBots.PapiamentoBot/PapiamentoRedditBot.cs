using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using RedditBots.Framework;
using RedditBots.PapiamentoBot.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.PapiamentoBot
{
    public class PapiamentoRedditBot : AbstractPapiamentoBot
    {
        protected RedditClient _redditClient;
        private readonly BotSetting _botSetting;

        public PapiamentoRedditBot(
                ILogger<PapiamentoRedditBot> logger,
                IHostEnvironment env,
                IOptions<MonitorSettings> monitorSettings,
                IOptions<PapiamentoBotSettings> papiamentoBotSettings)
        {
            Logger = logger;
            Env = env;
            _botSetting = monitorSettings.Value.Settings.Find(ms => ms.BotName == nameof(PapiamentoRedditBot)) ?? throw new ArgumentNullException("No bot settings found");
            PapiamentoBotSettings = papiamentoBotSettings.Value;

            _redditClient = new RedditClient(_botSetting.AppId, _botSetting.RefreshToken, _botSetting.AppSecret);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation($"Started {_botSetting.BotName} in {Env.EnvironmentName}");

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

                Logger.LogDebug($"Started monitoring {subredditToMonitor}");
            }
        }

        private void C_NewCommentsUpdated(object sender, CommentsUpdateEventArgs e)
        {
            foreach (Comment comment in e.Added)
            {
                if (comment.Author == _botSetting.BotName)
                {
                    continue;
                }

                Logger.LogDebug($"New comment detected of /u/{comment.Author} in /r/{comment.Subreddit}");

                _handleComment(comment);
            }
        }

        private void _handleComment(Comment comment)
        {
            if (string.Equals(comment.Author, "PapiamentoBot", StringComparison.OrdinalIgnoreCase))
            {
                // TODO check for compliment e.g. 'Good bot' under a comment by the bot

                return;
            }

            var response = CheckCommentGrammar(new Models.Request
            {
                Content = comment.Body,
                From = $"subreddit /r/{comment.Subreddit}"
            });

            if (response.MistakeFound)
            {
                comment.Reply(_buildCommentReply(comment, response.Mistake));
            }
        }

        private string _buildCommentReply(Comment comment, Word mistake)
        {
            var replyText = string.Format(_botSetting.DefaultReplyMessage, comment.Author, mistake.Wrong, mistake.Right);

            if (!string.IsNullOrWhiteSpace(mistake.Tip))
            {
                replyText += $"\n\n **Tip:** {mistake.Tip}";
            }

            Logger.LogInformation($"Writing reply to /u/{comment.Author} in /r/{comment.Subreddit} text: {replyText}");

            return replyText += _botSetting.MessageFooter;
        }
    }
}
