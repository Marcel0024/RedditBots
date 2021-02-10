using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedditBots.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using BackgroundService = RedditBots.Framework.BackgroundService;

namespace DiscordBots.FFBot
{
    public class FFBot : BackgroundService
    {
        private static DiscordSocketClient _discordClient;
        private readonly BotSetting _botSetting;
        private ILogger<FFBot> _logger;
        private readonly IHostEnvironment _env;

        private readonly Random _random = new Random();

        public FFBot(
                ILogger<FFBot> logger,
                IHostEnvironment env,
                IOptions<MonitorSettings> monitorSettings)
        {
            _logger = logger;
            _env = env;
            _botSetting = monitorSettings.Value.Settings.Find(ms => ms.BotName == nameof(FFBot)) ?? throw new ArgumentNullException("No bot settings found");

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

            if (message.Content == "atlas-weekly")
            {
                await message.Channel.SendMessageAsync("Atlas van de week is Raymond Benjamins", messageReference: new MessageReference(message.Id));
            }

            else if (message.Content == "atlas-monthly")
            {
                await message.Channel.SendMessageAsync("Atlas van de week is Thijs", messageReference: new MessageReference(message.Id));
            }

            else if (message.Content == "atlas-yearly")
            {
                await message.Channel.SendMessageAsync("ook Thijs", messageReference: new MessageReference(message.Id));
            }

            else if (_random.Next(0, 1000) < 7)
            {
                var random = _random.Next(0, 5);

                if (message.Content.StartsWith("pls porn", StringComparison.OrdinalIgnoreCase))
                {
                    await message.Channel.SendMessageAsync($"{message.Author.Mention}, Melvin vroeg als je hiermee kan stoppen", messageReference: new MessageReference(message.Id));
                }
                else if (random == 0)
                {
                    await message.Channel.SendMessageAsync($"{message.Author.Mention}, iemand vroeg waarom je altijd kut code schrijft", messageReference: new MessageReference(message.Id));
                }
                else if (random == 1)
                {
                    await message.Channel.SendMessageAsync($"{message.Author.Mention}, FYI workitems verwijderen is verboden.", messageReference: new MessageReference(message.Id));
                }
                else if (random == 2)
                {
                    await message.Channel.SendMessageAsync($"{message.Author.Mention}, onthou we hebben een staging omgeving niet voor niks.", messageReference: new MessageReference(message.Id));
                }
                else
                {
                    await message.Channel.SendMessageAsync($"GVD {message.Author.Mention}, ga eens aan het werk", messageReference: new MessageReference(message.Id));
                }
            }
        }
    }
}
