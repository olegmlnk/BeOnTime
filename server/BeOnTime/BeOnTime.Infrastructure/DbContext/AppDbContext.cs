using Microsoft.EntityFrameworkCore;

namespace BeOnTime.Infrastructure.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
 public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
 {
 }  
 
}