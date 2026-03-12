using Microsoft.EntityFrameworkCore;

namespace BeOnTime.Infrastructure.DbContext;

public class AppDbContext : DbContext
{
 public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
 {
 }  
 
}