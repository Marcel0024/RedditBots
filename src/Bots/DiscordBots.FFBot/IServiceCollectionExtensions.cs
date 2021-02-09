using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBots.FFBot
{
    public static class IServiceCollectionExtensions
    {
        public static void AddFFDiscordBot(this IServiceCollection services)
        {
            services.AddHostedService<FFBot>();
        }
    }
}
