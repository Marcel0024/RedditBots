using RedditBots.PapiamentoBot.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBots.PapiamentoBot.Models
{
    public class Response
    {
        public static Response Empty() => new Response();

        public bool MistakeFound { get; set; }
        public Word Mistake { get; set; }
    }
}
