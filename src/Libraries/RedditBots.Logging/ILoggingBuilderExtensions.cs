using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System;

namespace RedditBots.Logging
{
    public static class ILoggingBuilderExtensions
    {
        public static ILoggingBuilder AddUrl(this ILoggingBuilder builder, IConfiguration config)
        {
            builder.Services.TryAddSingleton<UrlLoggerQueue>();
            builder.Services.AddHostedService<UrlLoggerProcessor>();
            builder.Services.Configure<UrlLoggerOptions>(config.GetSection("Logging:Url"));
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, UrlLoggerProvider>());
            builder.Services.AddHttpClient<UrlLoggerService>(options => options.Timeout = TimeSpan.FromSeconds(3));

            LoggerProviderOptions.RegisterProviderOptions<UrlLoggerOptions, UrlLoggerProvider>(builder.Services);

            builder.Services.RemoveAll<IHttpMessageHandlerBuilderFilter>();

            return builder;
        }
    }
}
