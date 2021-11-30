using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using RedditBots.Libraries.BotFramework;
using System;
using System.Text;

namespace RedditBots.Bots.CheerfulBot
{
    public class CheerfulBot : RedditBotBackgroundService
    {
        protected override bool MonitorPosts => false;
        protected override bool MonitorComments => true;
        private readonly Random _random = new(69);

        private readonly string[] _quotes = new string[] {
            "Don’t give up when dark times come. The more storms you face in life, the stronger you’ll be. Hold on. Your greater is coming.",
            "The sky is everywhere, it begins at your feet.",
            "Stop feeling sorry for yourself and you will be happy.",
            "I’ve got nothing to do today but smile.",
            "Nobody is superior, nobody is inferior, but nobody is equal either. People are simply unique, incomparable. You are you, I am I.",
            "Let us be of good cheer, however, remembering that the misfortunes hardest to bear are those which never come.",
            "Laugh now, cry later.",
            "Cheer up, my dear. After every storm comes the sun. Happiness is waiting for you ahead.",
            "Once they move, they’re gone. Once you move, life starts over again.",
            "It’s better to have loved and lost than never to have loved at all.",
            "What doesn’t kill you makes you stronger.",
            "My entire life can be described in one sentence: It didn’t go as planned, and that’s okay."
        };

        public CheerfulBot(ILogger<CheerfulBot> logger, IOptions<MonitorSettings> monitorSettings) 
            : base(logger, monitorSettings)
        { }

        protected override void C_NewPostsUpdated(object sender, PostsUpdateEventArgs e)
        {
            return;
        }

        protected override void C_NewCommentsUpdated(object _, CommentsUpdateEventArgs e)
        {
            foreach (Comment comment in e.Added)
            {
                Logger.LogDebug($"New comment detected of /u/{comment.Author} in /r/{comment.Subreddit}");

                if (comment.Depth > 2)
                {
                    continue;
                }

                if (comment.Body.StartsWith("I'm sad that", StringComparison.OrdinalIgnoreCase)
                    && comment.Body.Length < 50)
                {
                    BuildReplyComment(comment);
                }
            }
        }

        private void BuildReplyComment(Comment comment)
        {
            Logger.LogInformation($"Sad user ({comment.Author}) detected in /r/{comment.Subreddit}, leaving cheerful comment");

            StringBuilder builder = new();

            builder
                .Append("> I'm sad that ...")
                .Append("\n\n")
                .Append(string.Format(BotSetting.DefaultReplyMessage, comment.Author))
                .Append("\n\n")
                .Append($"> *{GetRandomQuote()}*")
                .Append("\n\n")
                .Append(BotSetting.MessageFooter);

            comment.Reply(builder.ToString());
        }

        private string GetRandomQuote()
        {
            return _quotes[_random.Next(_quotes.Length - 1)];
        }
    }
}
