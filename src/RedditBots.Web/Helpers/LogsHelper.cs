using RedditBots.Web.Models;
using System;
using System.Collections.Generic;

namespace RedditBots.Web.Helpers
{
    public class LogsHelper
    {
        private const int _totalLogsHistory = 20;

        public DateTime? LastLogDateTime { get; set; }

        public List<LogEntry> LastLogs { get; set; } = new List<LogEntry>(_totalLogsHistory);

        internal void Log(LogEntry entry)
        {
            LastLogDateTime = DateTime.Now;

            if (LastLogs.Count == _totalLogsHistory)
            {
                LastLogs.RemoveAt(0);
            }

            LastLogs.Add(entry);
        }
    }
}
