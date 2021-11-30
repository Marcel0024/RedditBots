
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedditBots.Bots.PapiamentoBot.Models;
using RedditBots.Bots.PapiamentoBot.Settings;
using System;
using System.Linq;

namespace RedditBots.Bots.PapiamentoBot.Services
{
    public class PapiamentoService
    {
        private static readonly char[] _charactersToTrim = new char[] { '?', '.', ',', '!', ' ', '“', '”', '‘', '(', ')' };
        private readonly ILogger<PapiamentoService> _logger;
        private readonly PapiamentoBotSettings _papiamentoBotSettings;

        public PapiamentoService(ILogger<PapiamentoService> logger, IOptions<PapiamentoBotSettings> papiamentoBotSettings)
        {
            _logger = logger;
            _papiamentoBotSettings = papiamentoBotSettings.Value;
        }

        /// <summary>
        /// Checks if the comment is eligible for reply 
        /// </summary>
        internal Response CheckCommentGrammar(Request request)
        {
            var allWords = request.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (allWords.Length <= 2)
            {
                return Response.Empty();
            }

            if (!VerifyLanguage(allWords))
            {
                return Response.Empty();
            }

            _logger.LogInformation($"Verified papiamento in {request.From}: \"{request.Content}\"");

            if (!ContainsGrammarMistake(allWords, out Word mistake))
            {
                return Response.Empty();
            }

            return new Response
            {
                Mistake = mistake
            };
        }

        private bool VerifyLanguage(string[] allWords)
        {
            double totalMatchingWords = allWords.Count(commentWord =>
            {
                var word = commentWord.Trim(_charactersToTrim).ToLowerInvariant();

                return _papiamentoBotSettings.WordsToDetectLanguage.Contains(word)
                    || _papiamentoBotSettings.WordsToCorrect.Any(wtc => wtc.Wrong.ToLowerInvariant() == word || wtc.Right.ToLowerInvariant() == word)
                    || _papiamentoBotSettings.WordsToDetectLanguage.Any(wtl => wtl + "nan" == word);
            });

            // Language is verified if more then LanguageDetectionPercentage (percentage) of the words match the know words
            var percentageMatchWords = totalMatchingWords * 100 / allWords.Length;
            var percentageRounded = Math.Round(percentageMatchWords, 2, MidpointRounding.AwayFromZero).ToString("0.00");

            if (percentageMatchWords <= _papiamentoBotSettings.LanguageDetectionPercentage)
            {
                return false;
            }
            else
            {
                _logger.LogDebug($"Papiamento detected with {percentageRounded}% of {allWords.Length} words. Threshold: {_papiamentoBotSettings.LanguageDetectionPercentage}%.");
                return true;
            }
        }

        /// <summary>
        /// Checks if any mistake is detected in the array
        /// </summary>
        private bool ContainsGrammarMistake(string[] allWords, out Word mistake)
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
