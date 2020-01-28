using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedditBots.Bots;
using RedditBots.Logging;
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
            .ConfigureLogging((hostingContext, loggingBuilder) =>
            {
                loggingBuilder.AddUrl(hostingContext.Configuration);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<MonitorSettings>(hostContext.Configuration.GetSection(nameof(MonitorSettings)));
                services.Configure<PapiamentoBotSettings>(hostContext.Configuration.GetSection(nameof(PapiamentoBotSettings)));

                services.AddHostedService<PapiamentoBot>();
                services.AddHostedService<HanzeMemesBot>();
            });
    }
}