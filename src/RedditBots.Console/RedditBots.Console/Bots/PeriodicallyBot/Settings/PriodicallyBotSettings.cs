using System;
using System.Collections.Generic;

namespace RedditBots.Console.Bots.PeriodicallyBot.Settings;

public class PeriodicallyBotSettings
{
    public TimeSpan TimeOfExecution { get; set; } = TimeSpan.FromHours(13);

    public List<PeriodicTask> PeriodicTasks { get; set; } = new List<PeriodicTask>();
}

public class PeriodicTask
{
    public int DayOfTheMonth { get; set; }

    public TaskType TaskType { get; set; }
}

public enum TaskType
{
    PostToCSharpMonthlyThread
}

