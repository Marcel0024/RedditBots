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
        private readonly ILogger<FFBot> _logger;
        private readonly IHostEnvironment _env;

        private readonly Random _random = new Random();

        private readonly string[] _randomMessages = new[]
        {
            "{0}, iemand vroeg waarom je altijd kut code schrijft",
            "Fun fact: Workitems verwijderen is verboden",
            "{0}, onthou we hebben niet voor niks een staging omgeving",
            "{0}, het is normaal om huilend op de fiets naar huis te gaan",
            "{0}, vergeet je uren niet bij te werken op devops!",
            "Bruinester = racist",
            "Fun fact: Klantentest is niet een officiele term",
            "Fun fact: Je mag altijd gaan binden.",
            "{0}, waarom komt jou naam naar boven bij elke git blame?",
            "GVD {0}, ga eens aan het werk",
            "GVD {0}, ga eens aan het werk",
            "GVD {0}, ga eens aan het werk",
            "Wat snap je nou weer niet {0}",
            "Ik zie dat je 5 keer 'hoe werkt een array' heb gegoogled, dit kan ECHT niet",
            "Fact: Gezelligheid op kantoor is verboden",
            "Fact: Je moet altijd 110% geven",
            "Fact: Fotofabriek heeft de beste IT team van Groningen."
        };

        public FFBot(
            ILogger<FFBot> logger,
            IHostEnvironment env,
            IOptions<MonitorSettings> monitorSettings)
        {
            _logger = logger;
            _env = env;
            _botSetting = monitorSettings.Value.Settings.Find(ms => ms.BotName == nameof(FFBot));
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
            if (string.IsNullOrWhiteSpace(message.Content))
            {
                return;
            }

            if (message.Author.IsBot)
            {
                return;
            }

            var commands = message.Content.Split(' ');

            if (commands.Length > 1 && commands[0].StartsWith("ff", StringComparison.OrdinalIgnoreCase))
            {
                if (commands[1] == "atlas")
                {
                    await message.Channel.SendMessageAsync("Atlas van de week is Raymond Benjamins", messageReference: new MessageReference(message.Id));
                }
                else if (commands[1] == "help")
                {
                    if (_random.Next(0, 5) < 1)
                    {
                        await message.Channel.SendMessageAsync($"Dit bot doet niks, zoals jij, {message.Author.Mention}", messageReference: new MessageReference(message.Id));
                    }
                    else
                    {
                        var embedBuild = new EmbedBuilder
                        {
                            Title = "Commands",
                            Footer = new EmbedFooterBuilder
                            {
                                Text = "Dit bot is niet van Fotofabriek"
                            }
                        };

                        embedBuild.AddField("Atlas van de week", "ff atlas");

                        await message.Channel.SendMessageAsync(embed: embedBuild.Build());
                    }
                }
            }

            else if (_random.Next(0, 200) < 1)
            {
                if (message.Content.StartsWith("pls porn", StringComparison.OrdinalIgnoreCase))
                {
                    await message.Channel.SendMessageAsync($"{message.Author.Mention}, Melvin vroeg als je hiermee kan stoppen", messageReference: new MessageReference(message.Id));
                }
                else
                {
                    var random = _random.Next(0, _randomMessages.Length);
                    var replyMessage = string.Format(_randomMessages[random], message.Author.Mention);

                    var messageReference = _randomMessages[random].Contains("{0}")
                        ? new MessageReference(message.Id)
                        : null;

                    _logger.LogInformation($"replying in {message.Channel.Name}: {replyMessage}");

                    await message.Channel.SendMessageAsync(replyMessage, messageReference: messageReference);
                }
            }
        }
    }
}
