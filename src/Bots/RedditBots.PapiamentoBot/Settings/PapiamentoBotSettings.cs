using System.Collections.Generic;

namespace RedditBots.PapiamentoBot.Settings
{
    public class PapiamentoBotSettings
    {
        public int LanguageDetectionPercentage { get; set; }

        public IEnumerable<string> WordsToDetectLanguage { get; set; }

        public List<Word> WordsToCorrect { get; set; }
    }

    public class Word
    {
        public double Gravity { get; set; }

        public string Wrong { get; set; }

        public string Right { get; set; }

        public string Tip { get; set; }
    }
}
