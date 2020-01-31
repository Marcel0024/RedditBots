using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System.Net.Http;

namespace RedditBots.Libraries.Logging
{
    public static class ILoggingBuilderExtensions
    {
        public static ILoggingBuilder AddHttp(this ILoggingBuilder builder)
        {
            builder.Services.TryAddSingleton<HttpLoggerQueue>();
            builder.Services.AddHostedService<HttpLoggerProcessor>();
            builder.Services.AddHttpClient<HttpLoggerService>();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, HttpLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<HttpLoggerOptions, HttpLoggerProvider>(builder.Services);

            builder.AddFilter<HttpLoggerProvider>($"{typeof(HttpClient).FullName}.{nameof(HttpLoggerService)}", LogLevel.None);

            return builder;
        }
    }
}
