using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using System;
using Microsoft.Extensions.Configuration;

namespace RedditBots.Logging
{
    public static class ILoggingBuilderExtensions
    {
        public static ILoggingBuilder AddRedditBots(this ILoggingBuilder builder, IConfiguration config)
        {
            builder.Services.AddHttpClient<RedditBotsLoggerService>(options => options.Timeout = TimeSpan.FromSeconds(3));
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, RedditBotsLoggerProvider>());
            builder.Services.TryAddSingleton<RedditBotsLogsQueue>();
            LoggerProviderOptions.RegisterProviderOptions<RedditBotsLoggerOptions, RedditBotsLoggerProvider>(builder.Services);

            builder.Services.Configure<RedditBotsLoggerOptions>(config.GetSection("RedditBotsLogging"));

            builder.Services.AddHostedService<RedditBotsLoggingProcessor>();

            return builder;
        }
    }
}
