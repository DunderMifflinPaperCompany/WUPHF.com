using Microsoft.EntityFrameworkCore;
using WuphfWeb.Models;

namespace WuphfWeb.Data
{
    public class WuphfDbContext : DbContext
    {
        public WuphfDbContext(DbContextOptions<WuphfDbContext> options) : base(options)
        {
        }

        public DbSet<Wuphf> Wuphfs { get; set; }
        public DbSet<WuphfReply> WuphfReplies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<NotificationChannel> NotificationChannels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wuphf>()
                .Property(w => w.NotificationChannels)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            modelBuilder.Entity<WuphfReply>()
                .HasOne(r => r.Wuphf)
                .WithMany(w => w.Replies)
                .HasForeignKey(r => r.WuphfId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
