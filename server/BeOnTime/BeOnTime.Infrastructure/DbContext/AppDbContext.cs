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
    }); modelBuilder.Entity<RefreshToken>(entity =>
    {
     entity.HasOne(rt => rt.User)
      .WithMany()
      .HasForeignKey(rt => rt.UserId);
    });
   }
 }
 