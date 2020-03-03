using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using RedditBots.Framework;
using RedditBots.PapiamentoBot.Settings;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackgroundService = RedditBots.Framework.BackgroundService;

namespace RedditBots.PapiamentoBot
{
    /// <summary>
    /// PapiamentoBot monitors all new comments and check if a grammer mistake has been made.
    /// If so reply with a correction
    /// </summary>
    public class PapiamentoBot : BackgroundService
    {
        private readonly ILogger<PapiamentoBot> _logger;
        private readonly IHostEnvironment _env;
        private readonly RedditClient _redditClient;
        private readonly BotSetting _botSetting;
        private readonly PapiamentoBotSettings _papiamentoBotSettings;

        private static readonly char[] _charactersToTrim = new char[] { '?', '.', ',', '!', ' ', '“', '”', '‘', '(', ')' };

        public PapiamentoBot(
            ILogger<PapiamentoBot> logger,
            IHostEnvironment env,
            IOptions<MonitorSettings> monitorSettings,
            IOptions<PapiamentoBotSettings> papiamentoBotSettings)
        {
            _logger = logger;
            _env = env;
            _botSetting = monitorSettings.Value.Settings.Find(ms => ms.BotName == nameof(PapiamentoBot)) ?? throw new ArgumentNullException("No bot settings found");
            _papiamentoBotSettings = papiamentoBotSettings.Value;

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

                _logger.LogDebug($"Started monitoring {subredditToMonitor}");
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

                _logger.LogDebug($"New comment detected of /u/{comment.Author} in /r/{comment.Subreddit}");

                _handleComment(comment);
            }
        }

        private void _handleComment(Comment comment)
        {
            if (string.Equals(comment.Author, _botSetting.BotName, StringComparison.OrdinalIgnoreCase))
            {
                // TODO check for compliment e.g. 'Good bot' under a comment by the bot

                return;
            }

            _checkCommentGrammar(comment);
        }

        /// <summary>
        /// Checks if the comment is eligible for reply 
        /// If so write reply to the author, otherwise do nothing
        /// </summary>
        private void _checkCommentGrammar(Comment comment)
        {
            var allWords = comment.Body.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (allWords.Count() <= 2)
            {
                return;
            }

            if (!_verifyLanguage(allWords))
            {
                return;
            }

            _logger.LogInformation($"Verified papiamento: \"{comment.Body}\"");

            if (!_containsGrammarMistake(allWords, out Word mistake))
            {
                return;
            }

            var replyText = string.Format(_botSetting.DefaultReplyMessage, comment.Author, mistake.Wrong, mistake.Right);

            if (!string.IsNullOrWhiteSpace(mistake.Tip))
            {
                replyText += $"\n\n **Tip:** {mistake.Tip}";
            }

            _logger.LogInformation($"Writing reply to /u/{comment.Author} in /r/{comment.Subreddit} text: {replyText}");

            comment.Reply(replyText += _botSetting.MessageFooter);
        }

        private bool _verifyLanguage(string[] allWords)
        {
            double totalMatchingWords = allWords.Count(commentWord =>
            {
                var word = commentWord.Trim(_charactersToTrim).ToLowerInvariant();

                return _papiamentoBotSettings.WordsToDetectLanguage.Contains(word)
                    || _papiamentoBotSettings.WordsToCorrect.Any(wtc => wtc.Wrong.ToLowerInvariant() == word || wtc.Right.ToLowerInvariant() == word)
                    || _papiamentoBotSettings.WordsToDetectLanguage.Any(wtl => wtl + "nan" == word);
            });

            // Language is verified if more then LanguageDetectionPercentage (percentage) of the words match the know words
            var percentageMatchWords = totalMatchingWords * 100 / allWords.Count();

            if (percentageMatchWords <= _papiamentoBotSettings.LanguageDetectionPercentage)
            {
                return false;
            }

            var percentageRounded = Math.Round(percentageMatchWords, 2, MidpointRounding.AwayFromZero).ToString("0.00");

            _logger.LogDebug($"Papiamento detected with {percentageRounded}% of {allWords.Count()} words, checking for grammar mistakes");

            return true;
        }

        /// <summary>
        /// Checks if any mistake is detected in the array
        /// </summary>
        private bool _containsGrammarMistake(string[] allWords, out Word mistake)
        {
            mistake = null;

            foreach (var word in _papiamentoBotSettings.WordsToCorrect)
            {
                if (allWords.Any(w => w.Trim(_charactersToTrim).ToLowerInvariant() == word.Wrong))
                {
                    if (mistake == null || word.Gravity < mistake.Gravity)
                    {
                        mistake = word;
                    }
                }
            }

            if (mistake != null)
            {
                _logger.LogInformation($"Grammar mistake found: {mistake.Wrong}");

                return true;
            }

            _logger.LogInformation($"No grammar mistake found");

            return false;
        }
    }
}
