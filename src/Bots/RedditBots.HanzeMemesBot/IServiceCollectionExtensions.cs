using Microsoft.Extensions.DependencyInjection;

namespace RedditBots.HanzeMemesBot
{
    public static class IServiceCollectionExtensions
    {
        public static void AddHanzeMemesBot(this IServiceCollection services)
        {
            services.AddHostedService<HanzeMemesBot>();
        }
    }
}
