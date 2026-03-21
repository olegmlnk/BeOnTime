using BeOnTime.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace BeOnTime.Infrastructure.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
 public DbSet<User> Users { get; set; }
 public DbSet<Task> Tasks { get; set; }
 public DbSet<Idea> Ideas { get; set; }
 public DbSet<Reminding> Remindings { get; set; }
 public DbSet<Roadmap> Roadmaps { get; set; }
 public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}