using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedditBots.Console.Bots;
using RedditBots.Libraries.Logging;
using RedditBots.Settings;

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
            })
            .ConfigureLogging((loggingBuilder) =>
            {
                loggingBuilder.AddHttp();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<MonitorSettings>(hostContext.Configuration.GetSection(nameof(MonitorSettings)));
                services.Configure<PapiamentoBotSettings>(hostContext.Configuration.GetSection(nameof(PapiamentoBotSettings)));

                services.AddHostedService<PapiamentoBot>();
                services.AddHostedService<HanzeMemesBot>();
                services.AddHostedService<CheerfulBot>();
            });
    }
}