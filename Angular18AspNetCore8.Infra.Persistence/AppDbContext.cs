using Angular18AspNetCore8.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Angular18AspNetCore8.Infra.Persistence;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
  public DbSet<TodoTask> TodoTasks { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    base.OnModelCreating(modelBuilder);
  }
}
