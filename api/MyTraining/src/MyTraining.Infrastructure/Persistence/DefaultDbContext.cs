using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyTraining.Core.Entities;
using MyTraining.Core.Interfaces.Persistence;

namespace MyTraining.Infrastructure.Persistence;

public class DefaultDbContext : DbContext, IUnitOfWork
{

    public DefaultDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = default!;
    
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