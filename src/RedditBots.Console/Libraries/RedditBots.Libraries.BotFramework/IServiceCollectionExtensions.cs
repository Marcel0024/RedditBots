using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RedditBots.Libraries.BotFramework
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRedditBotFramework(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MonitorSettings>(configuration.GetSection(nameof(MonitorSettings)));
            return services;
        }
    }
}
