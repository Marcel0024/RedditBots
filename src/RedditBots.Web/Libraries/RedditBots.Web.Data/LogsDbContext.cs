using Microsoft.EntityFrameworkCore;
using RedditBots.Web.Data.Models;

namespace RedditBots.Web.Data
{
    public class LogsDbContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }

        public LogsDbContext(DbContextOptions<LogsDbContext> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
