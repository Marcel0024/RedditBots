using RedditBots.PapiamentoBot.Settings;

namespace RedditBots.PapiamentoBot.Models
{
    public class Response
    {
        public static Response Empty() => new();

        public bool MistakeFound { get; set; }

        public Word Mistake { get; set; }
    }
}
