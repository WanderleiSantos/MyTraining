using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Interfaces.Persistence;

namespace Infrastructure.Persistence;

public class DefaultDbContext : DbContext, IUnitOfWork
{

    public DefaultDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Exercise> Exercises { get; set; } = default!;
    public DbSet<TrainingSheet> TrainingSheets { get; set; } = default!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public async Task<bool> CommitAsync()
    {
        var success = await SaveChangesAsync() > 0;

        // Possibility to dispatch domain events, etc

        return success;
    }

    public Task RollbackAsync()
    {
        return Task.CompletedTask;
    }
}