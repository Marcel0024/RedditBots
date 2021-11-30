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
        private readonly PapiamentoBotSettings _papiamentoBotSettings;

        public PapiamentoService(IOptions<PapiamentoBotSettings> papiamentoBotSettings)
        {
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

            if (!VerifyLanguage(allWords, out string percentagePapiamento))
            {
                return Response.Empty();
            }


            if (!ContainsGrammarMistake(allWords, out Word mistake))
            {
                return Response.Empty();
            }

            return new Response
            {
                Mistake = mistake,
                PercentagePapiamento = percentagePapiamento
            };
        }

        private bool VerifyLanguage(string[] allWords, out string percentageRounded)
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
            percentageRounded = Math.Round(percentageMatchWords, 2, MidpointRounding.AwayFromZero).ToString("0.00");

            if (percentageMatchWords <= _papiamentoBotSettings.LanguageDetectionPercentage)
            {
                return false;
            }
            else
            {
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
                return true;
            }

            return false;
        }
    }
}
