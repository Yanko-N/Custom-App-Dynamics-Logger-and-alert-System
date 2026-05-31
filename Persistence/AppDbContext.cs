using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<CustomLog> Logs { get; set; }
        public DbSet<Hook> Hooks { get; set; }
        public DbSet<HookEvent> HookEvents { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<AlertTrigger> AlertTriggers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiKey>()
                .HasIndex(k => k.KeyHash)
                .IsUnique();

            modelBuilder.Entity<CustomLog>()
                .HasIndex(l => new { l.ServiceId, l.Timestamp });

            modelBuilder.Entity<CustomLog>()
                .HasIndex(l => l.TraceId);

            modelBuilder.Entity<HookEvent>()
                .HasIndex(e => new { e.HookId, e.Status });

            modelBuilder.Entity<AlertTrigger>()
                .HasIndex(t => new { t.AlertId, t.FiredAt });
        }
    }
}