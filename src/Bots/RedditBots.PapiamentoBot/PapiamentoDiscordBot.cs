using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedditBots.Framework;
using RedditBots.PapiamentoBot.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.PapiamentoBot
{
    public class PapiamentoDiscordBot : AbstractPapiamentoBot
    {
        private static DiscordSocketClient _discordClient;
        private readonly BotSetting _botSetting;

        public PapiamentoDiscordBot(
                ILogger<AbstractPapiamentoBot> logger,
                IHostEnvironment env,
                IOptions<MonitorSettings> monitorSettings,
                IOptions<PapiamentoBotSettings> papiamentoBotSettings)
        {
            Logger = logger;
            Env = env;
            _botSetting = monitorSettings.Value.Settings.Find(ms => ms.BotName == nameof(PapiamentoDiscordBot)) ?? throw new ArgumentNullException("No bot settings found");
            PapiamentoBotSettings = papiamentoBotSettings.Value;

            _discordClient = new DiscordSocketClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _discordClient.LoginAsync(TokenType.Bot, _botSetting.AppSecret);
            await _discordClient.StartAsync();

            _discordClient.MessageReceived += C_NewCommentsUpdated;
        }

        private async Task C_NewCommentsUpdated(SocketMessage message)
        {
            if (message.Author.IsBot)
            {
                return;
            }

            var response = CheckCommentGrammar(new Models.Request
            {
                Content = message.Content,
                From = $"discord channel {message.Channel.Name}"
            });

            if (response.MistakeFound)
            {
                var replyText = string.Format(_botSetting.DefaultReplyMessage, message.Author, response.Mistake.Wrong, response.Mistake.Right);

                await message.Channel.SendMessageAsync(replyText);
            }
        }
    }
}
