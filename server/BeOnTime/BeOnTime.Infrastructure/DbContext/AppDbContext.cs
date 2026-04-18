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

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.UserName).IsUnique();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId);
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

        modelBuilder.Entity<Reminding>(entity =>
        {
            entity.Property(r => r.Message).HasMaxLength(500);
            entity.Property(r => r.Status).HasConversion<string>().HasMaxLength(32);

            entity.HasOne(r => r.Task)
                .WithMany()
                .HasForeignKey(r => r.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(r => new { r.Status, r.RemindAt });
            entity.HasIndex(r => r.UserId);

            entity.HasQueryFilter(r => r.DeletedAt == null);
        });
    }
}
