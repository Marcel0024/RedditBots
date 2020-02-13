﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedditBots.CheerfulBot;
using RedditBots.Framework;
using RedditBots.HanzeMemesBot;
using RedditBots.Libraries.Logging;
using RedditBots.PapiamentoBot;
using RedditBots.PriodicallyBot;

namespace RedditBots
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(configHost =>
            {
                configHost.AddJsonFile(@"Settings\papiamentobotsettings.json");
                configHost.AddJsonFile(@"Settings\periodicallybotsettings.json");
            })
            .ConfigureLogging((loggingBuilder) =>
            {
                loggingBuilder.AddHttp();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<MonitorSettings>(hostContext.Configuration.GetSection(nameof(MonitorSettings)));

                services.AddHanzeMemesBot();
                services.AddPriodicallyBot(hostContext);
                services.AddPapiamentoBot(hostContext);
            });
    }
}