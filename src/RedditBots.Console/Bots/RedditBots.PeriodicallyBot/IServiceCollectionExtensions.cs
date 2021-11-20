using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedditBots.PeriodicallyBot.Settings;

namespace RedditBots.PeriodicallyBot;

public static class IServiceCollectionExtensions
{
    public static void AddPeriodicallyBot(this IServiceCollection services, HostBuilderContext hostContext)
    {
        services.AddHostedService<PeriodicallyBot>();

        services.Configure<PeriodicallyBotSettings>(hostContext.Configuration.GetSection(nameof(PeriodicallyBotSettings)));
    }
}
