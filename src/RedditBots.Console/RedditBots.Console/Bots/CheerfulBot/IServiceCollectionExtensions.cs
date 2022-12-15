using Microsoft.Extensions.DependencyInjection;

namespace RedditBots.Console.Bots.CheerfulBot
{
    public static class IServiceCollectionExtensions
    {
        public static void AddCheerfulBot(this IServiceCollection services)
        {
            services.AddHostedService<CheerfulBot>();
        }
    }
}
