using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedditBots.Console.Bots.PapiamentoBot.Services;
using RedditBots.Console.Bots.PapiamentoBot.Settings;

namespace RedditBots.Console.Bots.PapiamentoBot;

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
