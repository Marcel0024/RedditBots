using RedditBots.Web.Data.Enums;
using System;

namespace RedditBots.Web.Data.Models
{
    public class Log
    {
        public Guid Id { get; set; }

        public string Application { get; set; }

        public string Message { get; set; }

        public LogLevel Level { get; set; }

        public DateTime DateTime { get; set; }
    }
}
