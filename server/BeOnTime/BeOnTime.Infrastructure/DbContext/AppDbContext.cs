using BeOnTime.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace BeOnTime.Infrastructure.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Idea> Ideas { get; set; }
    public DbSet<Reminding> Remindings { get; set; }
    public DbSet<Roadmap> Roadmaps { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<UserSettingsHistory> UserSettingsHistories { get; set; }
    public DbSet<UserExportJob> UserExportJobs { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.UserName).IsUnique();
            entity.HasQueryFilter(e => e.DeletedAt == null);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(rt => rt.UserAgent).HasMaxLength(512);
            entity.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId);
            entity.HasIndex(rt => rt.UserId);
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Description).HasMaxLength(2000);
            entity.Property(t => t.Status).HasConversion<string>().HasMaxLength(32);
            entity.Property(t => t.Priority).HasConversion<string>().HasMaxLength(16);

            entity.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(t => new { t.UserId, t.Status });
            entity.HasIndex(t => new { t.UserId, t.Deadline });

            entity.HasQueryFilter(t => t.DeletedAt == null);
        });

        modelBuilder.Entity<Idea>(entity =>
        {
            entity.Property(i => i.Title).IsRequired().HasMaxLength(200);
            entity.Property(i => i.Content).IsRequired().HasMaxLength(5000);

            entity.HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(i => i.UserId);

            entity.HasQueryFilter(i => i.DeletedAt == null);
        });

        modelBuilder.Entity<Roadmap>(entity =>
        {
            entity.Property(r => r.Name).IsRequired().HasMaxLength(200);
            entity.Property(r => r.Description).HasMaxLength(2000);

            entity.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.Tasks)
                .WithOne()
                .HasForeignKey(t => t.RoadmapId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(r => r.UserId);

            entity.HasQueryFilter(r => r.DeletedAt == null);
        });

        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.Property(s => s.Theme).HasConversion<string>().HasMaxLength(16);
            entity.Property(s => s.Language).HasConversion<string>().HasMaxLength(8);
            entity.Property(s => s.TimeZone).IsRequired().HasMaxLength(64);
            entity.Property(s => s.DateFormat).HasConversion<string>().HasMaxLength(16);
            entity.Property(s => s.WeekStart).HasConversion<string>().HasMaxLength(16);
            entity.Property(s => s.DefaultTaskPriority).HasConversion<string>().HasMaxLength(16);

            entity.HasOne(s => s.User)
                .WithOne()
                .HasForeignKey<UserSettings>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(s => s.UserId).IsUnique();
        });

        modelBuilder.Entity<UserSettingsHistory>(entity =>
        {
            entity.Property(s => s.Theme).HasConversion<string>().HasMaxLength(16);
            entity.Property(s => s.Language).HasConversion<string>().HasMaxLength(8);
            entity.Property(s => s.TimeZone).IsRequired().HasMaxLength(64);
            entity.Property(s => s.DateFormat).HasConversion<string>().HasMaxLength(16);
            entity.Property(s => s.WeekStart).HasConversion<string>().HasMaxLength(16);
            entity.Property(s => s.DefaultTaskPriority).HasConversion<string>().HasMaxLength(16);

            entity.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(s => new { s.UserId, s.CreatedAt });
        });

        modelBuilder.Entity<UserExportJob>(entity =>
        {
            entity.Property(j => j.Status).HasConversion<string>().HasMaxLength(16);
            entity.Property(j => j.FileName).HasMaxLength(200);
            entity.Property(j => j.ContentType).HasMaxLength(100);
            entity.Property(j => j.ErrorMessage).HasMaxLength(2000);

            entity.HasOne(j => j.User)
                .WithMany()
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(j => new { j.UserId, j.CreatedAt });
            entity.HasIndex(j => j.Status);
        });
    }
}
