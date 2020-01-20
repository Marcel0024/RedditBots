using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedditBots.Bots;
using RedditBots.Settings;
using Reddit;

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
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<PapiamentoBot>();
                services.AddHostedService<FlairReminderBot>();

                services.Configure<MonitorSettings>(hostContext.Configuration.GetSection(nameof(MonitorSettings)));
                services.Configure<PapiamentoBotSettings>(hostContext.Configuration.GetSection(nameof(PapiamentoBotSettings)));
            });
    }
}