using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedditBots.PriodicallyBot.Settings;

namespace RedditBots.PriodicallyBot
{
    public static class IServiceCollectionExtensions
    {
        public static void AddPriodicallyBot(this IServiceCollection services, HostBuilderContext hostContext)
        {
            services.AddHostedService<PriodicallyBot>();

            services.Configure<PeriodicallyBotSettings>(hostContext.Configuration.GetSection(nameof(PeriodicallyBotSettings)));
        }
    }
}
