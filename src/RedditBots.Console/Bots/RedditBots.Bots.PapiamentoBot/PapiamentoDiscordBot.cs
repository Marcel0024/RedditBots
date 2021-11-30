using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedditBots.Bots.PapiamentoBot.Services;
using RedditBots.Libraries.BotFramework;
using System.Threading.Tasks;

namespace RedditBots.Bots.PapiamentoBot;

public class PapiamentoDiscordBot : DiscordBackgroundService
{
    private readonly PapiamentoService _papiamentoService;

    public PapiamentoDiscordBot(
            ILogger<PapiamentoDiscordBot> logger,
            IOptions<MonitorSettings> monitorSettings,
            PapiamentoService papiamentoService)
        : base(logger, monitorSettings)
    {
        _papiamentoService = papiamentoService;
    }

    protected override async Task C_NewCommentsUpdated(SocketMessage message)
    {
        if (message.Author.IsBot)
        {
            return;
        }

        var response = _papiamentoService.CheckCommentGrammar(new Models.Request
        {
            Content = message.Content,
            From = $"discord channel {message.Channel.Name}"
        });

        if (response.MistakeFound())
        {
            Logger.LogInformation($"Papiamento mistake found with {response.PercentagePapiamento}% words recognized. Leaving comment for word {response.Mistake.Wrong}.");
            var replyText = string.Format(BotSetting.DefaultReplyMessage, message.Author.Mention, response.Mistake.Wrong, response.Mistake.Right);

            await message.Channel.SendMessageAsync(replyText);
        }
    }
}
