using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedditBots.Bots.PeriodicallyBot.Settings;

namespace RedditBots.Bots.PeriodicallyBot;

public static class IServiceCollectionExtensions
{
    public static void AddPeriodicallyBot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<PeriodicallyBot>();

        services.Configure<PeriodicallyBotSettings>(configuration.GetSection(nameof(PeriodicallyBotSettings)));
    }
}
