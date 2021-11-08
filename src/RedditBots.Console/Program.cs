using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedditBots.Framework;
using RedditBots.Libraries.Logging;
using System.IO;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureHostConfiguration(configHost =>
    {
        configHost.AddJsonFile(Path.Combine("Settings", "papiamentobotsettings.json"));
        configHost.AddJsonFile(Path.Combine("Settings", "periodicallybotsettings.json"));
    })
    .ConfigureLogging((loggingBuilder) =>
    {
        loggingBuilder.AddHttp();
    });

builder.ConfigureServices((hostContext, services) =>
   {
       services.Configure<MonitorSettings>(hostContext.Configuration.GetSection(nameof(MonitorSettings)));

       services.AddHanzeMemesBot();
       services.AddPeriodicallyBot(hostContext);
       services.AddPapiamentoBot(hostContext);
       //services.AddCheerfulBot();
   });

builder.Build().Run();
