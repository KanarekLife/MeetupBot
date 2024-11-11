using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MeetupBot.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<MeetupGroup> MeetupGroups { get; protected set; }
    public DbSet<MeetupEvent> MeetupEvents { get; protected set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MeetupGroup>(x => x.HasKey(y => y.Url));
        modelBuilder.Entity<MeetupEvent>(x => x.HasKey(y => y.Url));
    }

    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            return new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite("Data Source=meetupbot.db").Options);
        }
    }
}