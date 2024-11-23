using CommitWhisperer.Models;
using Microsoft.EntityFrameworkCore;

namespace CommitWhisperer.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<CommitInfo> Commits { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) 
            {
                optionsBuilder.UseSqlite("Data Source=commits.db");
            }
        }
    }
}
