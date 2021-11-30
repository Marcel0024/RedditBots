using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.Libraries.BotFramework
{
    public abstract class DiscordBackgroundService : BackgroundService
    {
        protected readonly DiscordSocketClient DiscordClient;

        public DiscordBackgroundService(ILogger<DiscordBackgroundService> logger, IOptions<MonitorSettings> monitorSettings)
            : base(logger, monitorSettings)
        {
            DiscordClient = BotSetting.IsEnabled
                ? new DiscordSocketClient()
                : null;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DiscordClient.LoginAsync(TokenType.Bot, BotSetting.AppSecret);
            await DiscordClient.StartAsync();

            DiscordClient.MessageReceived += C_NewCommentsUpdated;
        }

        protected abstract Task C_NewCommentsUpdated(SocketMessage arg);
    }
}
