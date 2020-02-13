﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedditBots.PapiamentoBot.Settings;


namespace RedditBots.PapiamentoBot
{
    public static class IServiceCollectionExtensions
    {
        public static void AddPapiamentoBot(this IServiceCollection services, HostBuilderContext hostContext)
        {
            services.AddHostedService<PapiamentoBot>();

            services.Configure<PapiamentoBotSettings>(hostContext.Configuration.GetSection(nameof(PapiamentoBotSettings)));
        }
    }
}
