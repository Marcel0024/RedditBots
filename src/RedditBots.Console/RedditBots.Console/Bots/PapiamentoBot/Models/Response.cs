using RedditBots.Console.Bots.PapiamentoBot.Settings;

namespace RedditBots.Bots.PapiamentoBot.Models;

public class Response
{
    public static Response Empty() => new();

    public Word Mistake { get; set; }
    public string PercentagePapiamento { get; set; }

    public bool MistakeFound() => Mistake != null;
}
