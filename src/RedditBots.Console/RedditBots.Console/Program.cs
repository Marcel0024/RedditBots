using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RedditBots.Console.Bots.CheerfulBot;
using RedditBots.Console.Bots.HanzeMemesBot;
using RedditBots.Console.Bots.PapiamentoBot;
using RedditBots.Console.Bots.PeriodicallyBot;
using RedditBots.Libraries.BotFramework;
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
       services.AddRedditBotFramework(hostContext.Configuration);

       services.AddHanzeMemesBot();
       services.AddPeriodicallyBot(hostContext.Configuration);
       services.AddPapiamentoBot(hostContext.Configuration);
       services.AddCheerfulBot();
   });

builder.Build().Run();
