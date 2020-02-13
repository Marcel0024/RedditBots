using System.Collections.Generic;

namespace RedditBots.Framework
{
    public class MonitorSettings
    {
        public List<BotSetting> Settings { get; set; }
    }

    public class BotSetting
    {
        public string BotName { get; set; }

        public string AppSecret { get; set; }

        public string AppId { get; set; }

        public string RefreshToken { get; set; }

        public string MessageFooter { get; set; }

        public string DefaultReplyMessage { get; set; }

        public List<string> Subreddits { get; set; }
    }
}
