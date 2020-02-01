using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedditBots.Console.Bots;
using RedditBots.Console.Settings;
using RedditBots.Libraries.Logging;

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
                configHost.AddJsonFile("papiamentobotsettings.json");
                configHost.AddJsonFile("periodicallybotsettings.json");
            })
            .ConfigureLogging((loggingBuilder) =>
            {
                loggingBuilder.AddHttp();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<MonitorSettings>(hostContext.Configuration.GetSection(nameof(MonitorSettings)));
                services.Configure<PapiamentoBotSettings>(hostContext.Configuration.GetSection(nameof(PapiamentoBotSettings)));
                services.Configure<PeriodicallyBotSettings>(hostContext.Configuration.GetSection(nameof(PeriodicallyBotSettings)));

                services.AddHostedService<PapiamentoBot>();
                services.AddHostedService<HanzeMemesBot>();
                services.AddHostedService<CheerfulBot>();
                services.AddHostedService<PeriodicallyBot>();
            });
    }
}