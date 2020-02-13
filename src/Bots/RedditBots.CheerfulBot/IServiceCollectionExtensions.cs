using Microsoft.Extensions.DependencyInjection;

namespace RedditBots.CheerfulBot
{
    public static class IServiceCollectionExtensions
    {
        public static void AddCheerfulBot(this IServiceCollection services)
        {
            services.AddHostedService<CheerfulBot>();
        }
    }
}
