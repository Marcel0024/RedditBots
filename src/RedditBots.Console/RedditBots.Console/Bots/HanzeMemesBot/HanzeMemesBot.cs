using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using RedditBots.Libraries.BotFramework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.Console.Bots.HanzeMemesBot;

/// <summary>
/// HanzeMemesBot replies to all new post with a 'Reminder' message to set a flair
/// Than it checks if a flair has been added to new posts, if so it deletes its own 'Reminder' message
/// </summary>
public class HanzeMemesBot : RedditBotBackgroundService
{
    protected override bool MonitorPosts => true;
    protected override bool MonitorComments => false;

    public HanzeMemesBot(ILogger<HanzeMemesBot> logger, IOptions<MonitorSettings> monitorSettings)
        : base(logger, monitorSettings)
    { }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await base.ExecuteAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                foreach (var subreddit in MonitoringSubreddits)
                {
                    MonitorPostsForAddedFlair(subreddit);
                }
            }
            catch (Exception e) when (e.GetType().Name.StartsWith("Reddit"))
            {
                Logger.LogWarning($"Reddit threw {e.GetType().Name}");

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // wait, reddit is probably down
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    protected override void C_NewPostsUpdated(object sender, PostsUpdateEventArgs e)
    {
        foreach (var post in e.Added)
        {
            Logger.LogInformation($"New post from /u/{post.Author} in /r/{post.Subreddit} leaving comment");

            post.Reply(string.Format(BotSetting.DefaultReplyMessage, post.Author) + BotSetting.MessageFooter);
        }
    }

    protected override void C_NewCommentsUpdated(object sender, CommentsUpdateEventArgs e)
    { }

    private void MonitorPostsForAddedFlair(Subreddit subreddit)
    {
        var newPosts = subreddit.Posts.New;

        foreach (var newPost in newPosts)
        {
            if (!string.IsNullOrWhiteSpace(newPost.Listing.LinkFlairText))
            {
                CheckForReminderComment(newPost);
            }
        }
    }

    private void CheckForReminderComment(Post newPost)
    {
        var oldComments = newPost.Comments.Old;

        foreach (var oldComment in oldComments)
        {
            if (string.Equals(oldComment.Author, BotSetting.BotName, StringComparison.OrdinalIgnoreCase))
            {
                Logger.LogInformation($"Flair detected, removing own comment in post of /u/{newPost.Author}");

                oldComment.Delete();
            }
        }
    }
}
