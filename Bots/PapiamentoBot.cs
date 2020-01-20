using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedditBots.Settings;
using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.Bots
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
        private readonly MonitorSetting _monitorSettings;
        private readonly PapiamentoBotSettings _papiamentoBotSettings;
        private static readonly char[] _charactersToTrim = new char[] { '?', '.', ',', '!', ' ' };

        public PapiamentoBot(
            ILogger<PapiamentoBot> logger,
            IHostEnvironment env,
            IOptions<MonitorSettings> monitorSettings,
            IOptions<PapiamentoBotSettings> papiamentoBotSettings)
        {
            _logger = logger;
            _env = env;
            _monitorSettings = monitorSettings.Value.Settings.Find(ms => ms.Bot == nameof(PapiamentoBot)) ?? throw new ArgumentNullException("No bot settings found");
            _papiamentoBotSettings = papiamentoBotSettings.Value;

            _redditClient = new RedditClient(_monitorSettings.AppId, _monitorSettings.RefreshToken, _monitorSettings.AppSecret);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Started {_monitorSettings.BotName} in {_env.EnvironmentName}");

            //foreach (var subredditToMonitor in _monitorSettings.Subreddits)
            //{
            //    _logger.LogInformation($"Started monitoring {subredditToMonitor}");

            //    var subreddit = _redditClient.Subreddit(subredditToMonitor);

            //    subreddit.Comments.GetNew();
            //    subreddit.Comments.MonitorNew();
            //    subreddit.Comments.NewUpdated += C_NewCommentsUpdated;
            //}

            var subreddit = _redditClient.Subreddit("AskReddit");

            subreddit.Comments.GetNew();
            subreddit.Comments.MonitorNew();

            subreddit.Comments.NewUpdated += C_NewCommentsUpdated;

            return Task.CompletedTask;
        }

        private void C_NewCommentsUpdated(object sender, CommentsUpdateEventArgs e)
        {
            foreach (Comment comment in e.Added)
            {
                _logger.LogDebug($"{DateTime.Now} New comment detected of {comment.Author} in {comment.Subreddit}");

                _handleComment(comment);
            }
        }

        private void _handleComment(Comment comment)
        {
            // Look under the bot for "Good bot", then don't continue recursively under the bot
            if (string.Equals(comment.Author, _monitorSettings.BotName, StringComparison.OrdinalIgnoreCase))
            {
                //if (comment.Comments.GetComments().Any())
                //{
                //    await _processCommentsAsync(comment?.Data?.Replies?.Data?.Children, true);
                //}

                return;
            }

            //// Repeat recursive, unless looking for compliments
            //if (!lookForCompliments && comment?.Data?.Replies?.Data?.Children != null)
            //{
            //    await _processCommentsAsync(comment.Data.Replies.Data.Children);
            //}

            //// Skip deleted comments
            //if (string.IsNullOrEmpty(comment?.Data?.Body) || string.IsNullOrEmpty(comment?.Data?.Author))
            //{
            //    return;
            //}

            // Check if already processed
            //if (_processedComments.TryGetValue(comment.Data.Subreddit.ToLowerInvariant(), out ConcurrentBag<string> comments))
            //{
            //    if (comments.Any(c => c == comment.Data.Id))
            //    {
            //        return;
            //    }
            //}

            // Check comment if a reply is needed
            //if (lookForCompliments)
            //{
            //    await _lookForComplimentsAsync(comment.Data);
            //}
            //else
            //{
            _checkCommentGrammar(comment);
            //}
        }

        /// <summary>
        /// Checks if the comment is eligible for reply 
        /// If so write reply to the author, otherwise do nothing
        /// </summary>
        /// <param name="data">The reddit listing</param>
        private void _checkCommentGrammar(Comment comment)
        {
            var allWords = comment.Body.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (!_verifyLanguage(allWords))
            {
                return;
            }

            if (_canReply(comment, allWords, out string replyText))
            {
                _writeReplyAsync(comment, replyText);
            }
        }

        /// <summary>
        /// Tries to verify language
        /// </summary>
        private bool _verifyLanguage(string[] allWords)
        {
            double totalMatchingWords = allWords.Count(commentWord =>
            {
                var word = commentWord.Trim(_charactersToTrim).ToLowerInvariant();

                return _papiamentoBotSettings.WordsToDetectLanguage.Contains(word)
                || _papiamentoBotSettings.WordsToCorrect.Any(wtc => wtc.Wrong.ToLowerInvariant() == word
                || wtc.Right.ToLowerInvariant() == word);
            });

            // Language is verified if more then LanguageDetectionPercentage (percentage) of the words match the know words
            var percentageMatchWords = totalMatchingWords * 100 / allWords.Count();
            if (percentageMatchWords <= _papiamentoBotSettings.LanguageDetectionPercentage)
            {
                return false;
            }

            var percentageRounded = Math.Round(percentageMatchWords, 2, MidpointRounding.AwayFromZero).ToString("0.00");
            _logger.LogInformation($"{DateTime.Now} {percentageRounded}% of {allWords.Count()} words matched, checking for grammar mistakes");

            return true;
        }

        /// <summary>
        /// Checks if any mistake are present in allwords
        /// Returns the formated message to reply
        /// If no mistakes are found return null.
        /// </summary>
        private bool _canReply(Comment comment, string[] allWords, out string replyText)
        {
            Word mistake = null;
            replyText = "";

            foreach (var word in _papiamentoBotSettings.WordsToCorrect)
            {
                if (allWords.Any(w => w.Trim(_charactersToTrim).ToLowerInvariant() == word.Wrong))
                {
                    if (mistake == null || word.Gravity < mistake.Gravity)
                    {
                        mistake = word;
                    }
                }
            };

            if (mistake != null)
            {
                replyText = string.Format(_monitorSettings.DefaultReplyMessage, comment.Author, mistake.Wrong, mistake.Right);

                return true;
            }

            return false;
        }

        private void _writeReplyAsync(Comment comment, string text)
        {
            _logger.LogInformation($"{DateTime.Now} Writing reply to u/{comment.Author} in r/{comment.Subreddit} text: {text}", ConsoleColor.Magenta);

            comment.Reply(text += _monitorSettings.MessageFooter);
        }
    }
}
