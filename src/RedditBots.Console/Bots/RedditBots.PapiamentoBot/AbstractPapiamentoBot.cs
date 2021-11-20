using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedditBots.PapiamentoBot.Models;
using RedditBots.PapiamentoBot.Settings;
using System;
using System.Linq;
using BackgroundService = RedditBots.Framework.BackgroundService;

namespace RedditBots.PapiamentoBot;

/// <summary>
/// PapiamentoBot monitors all new comments and check if a grammer mistake has been made.
/// If so reply with a correction
/// </summary>
public abstract class AbstractPapiamentoBot : BackgroundService
{
    protected ILogger<AbstractPapiamentoBot> Logger;
    protected IHostEnvironment Env;

    protected PapiamentoBotSettings PapiamentoBotSettings;

    private static readonly char[] _charactersToTrim = new char[] { '?', '.', ',', '!', ' ', '“', '”', '‘', '(', ')' };

    /// <summary>
    /// Checks if the comment is eligible for reply 
    /// If so write reply to the author, otherwise do nothing
    /// </summary>
    protected Response CheckCommentGrammar(Request request)
    {
        var allWords = request.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (allWords.Length <= 2)
        {
            return Response.Empty();
        }

        if (!_verifyLanguage(allWords))
        {
            return Response.Empty();
        }

        Logger.LogInformation($"Verified papiamento in {request.From}: \"{request.Content}\"");

        if (!_containsGrammarMistake(allWords, out Word mistake))
        {
            return Response.Empty();
        }

        return new Response
        {
            MistakeFound = true,
            Mistake = mistake
        };
    }

    protected bool _verifyLanguage(string[] allWords)
    {
        double totalMatchingWords = allWords.Count(commentWord =>
        {
            var word = commentWord.Trim(_charactersToTrim).ToLowerInvariant();

            return PapiamentoBotSettings.WordsToDetectLanguage.Contains(word)
                || PapiamentoBotSettings.WordsToCorrect.Any(wtc => wtc.Wrong.ToLowerInvariant() == word || wtc.Right.ToLowerInvariant() == word)
                || PapiamentoBotSettings.WordsToDetectLanguage.Any(wtl => wtl + "nan" == word);
        });

        // Language is verified if more then LanguageDetectionPercentage (percentage) of the words match the know words
        var percentageMatchWords = totalMatchingWords * 100 / allWords.Count();

        if (percentageMatchWords <= PapiamentoBotSettings.LanguageDetectionPercentage)
        {
            return false;
        }

        var percentageRounded = Math.Round(percentageMatchWords, 2, MidpointRounding.AwayFromZero).ToString("0.00");

        Logger.LogDebug($"PapiamentoBot detected with {percentageRounded}% of {allWords.Count()} words, checking for grammar mistakes");

        return true;
    }

    /// <summary>
    /// Checks if any mistake is detected in the array
    /// </summary>
    protected bool _containsGrammarMistake(string[] allWords, out Word mistake)
    {
        mistake = null;

        foreach (var word in PapiamentoBotSettings.WordsToCorrect)
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
            Logger.LogInformation($"Grammar mistake found: {mistake.Wrong}");

            return true;
        }

        Logger.LogInformation($"No grammar mistake found");

        return false;
    }
}
