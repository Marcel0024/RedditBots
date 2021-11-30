using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using RedditBots.Libraries.BotFramework;
using RedditBots.Bots.PapiamentoBot.Services;
using RedditBots.Bots.PapiamentoBot.Settings;
using System;

namespace RedditBots.Bots.PapiamentoBot;

public class PapiamentoRedditBot : RedditBotBackgroundService
{
    private readonly PapiamentoService _papiamentoService;

    protected override bool MonitorPosts => false;
    protected override bool MonitorComments => true;

    public PapiamentoRedditBot(
        ILogger<PapiamentoRedditBot> logger, 
        IOptions<MonitorSettings> monitorSettings, 
        PapiamentoService papiamentoService)
        : base(logger, monitorSettings)
    {
        _papiamentoService = papiamentoService;
    }

    protected override void C_NewCommentsUpdated(object sender, CommentsUpdateEventArgs e)
    {
        foreach (var comment in e.Added)
        {
            if (comment.Author == BotSetting.BotName)
            {
                continue;
            }

            Logger.LogDebug($"New comment detected of /u/{comment.Author} in /r/{comment.Subreddit}");

            HandleComment(comment);
        }
    }

    private void HandleComment(Comment comment)
    {
        if (string.Equals(comment.Author, "PapiamentoBot", StringComparison.OrdinalIgnoreCase))
        {
            // TODO check for compliment e.g. 'Good bot' under a comment by the bot

            return;
        }

        var response = _papiamentoService.CheckCommentGrammar(new Models.Request
        {
            Content = comment.Body,
            From = $"subreddit /r/{comment.Subreddit}"
        });

        if (response.MistakeFound())
        {
            if (response.Mistake.Right.Contains("ñ") &&
                new Random().Next(0, 4) == 0) // Don't be too spammy
            {
                comment.Reply(BuildCommentReply(comment, response.Mistake));
            }
        }
    }

    private string BuildCommentReply(Comment comment, Word mistake)
    {
        var replyText = string.Format(BotSetting.DefaultReplyMessage, comment.Author, mistake.Wrong, mistake.Right);

        if (!string.IsNullOrWhiteSpace(mistake.Tip))
        {
            replyText += $"\n\n **Tip:** {mistake.Tip}";
        }

        Logger.LogInformation($"Writing reply to /u/{comment.Author} in /r/{comment.Subreddit} text: {replyText}");

        return replyText += BotSetting.MessageFooter;
    }

    protected override void C_NewPostsUpdated(object sender, PostsUpdateEventArgs e)
    { }
}
