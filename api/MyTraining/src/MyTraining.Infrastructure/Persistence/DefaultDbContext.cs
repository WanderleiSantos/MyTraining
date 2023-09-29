using Microsoft.EntityFrameworkCore;
using MyTraining.Core.Repositories;

namespace MyTraining.Infrastructure.Persistence;

public class DefaultDbContext : DbContext, IUnitOfWork
{

    public DefaultDbContext(DbContextOptions options) : base(options)
    {
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