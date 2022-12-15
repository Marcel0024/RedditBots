using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedditBots.Console.Bots.PeriodicallyBot.Settings;

namespace RedditBots.Console.Bots.PeriodicallyBot;

public static class IServiceCollectionExtensions
{
    public static void AddPeriodicallyBot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<PeriodicallyBot>();

        services.Configure<PeriodicallyBotSettings>(configuration.GetSection(nameof(PeriodicallyBotSettings)));
    }
}
