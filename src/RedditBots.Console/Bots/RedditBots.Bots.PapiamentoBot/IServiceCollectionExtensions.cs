using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedditBots.Bots.PapiamentoBot.Services;
using RedditBots.Bots.PapiamentoBot.Settings;

namespace RedditBots.Bots.PapiamentoBot;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddPapiamentoBot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<PapiamentoRedditBot>();
        services.AddHostedService<PapiamentoDiscordBot>();

        services.Configure<PapiamentoBotSettings>(configuration.GetSection(nameof(PapiamentoBotSettings)));

        services.AddSingleton<PapiamentoService>();

        return services;
    }
}
