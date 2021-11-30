using RedditBots.Bots.PapiamentoBot.Settings;

namespace RedditBots.Bots.PapiamentoBot.Models;

public class Response
{
    public static Response Empty() => new();

    public Word Mistake { get; set; }


    public bool MistakeFound() => Mistake != null;
}
