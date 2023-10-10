using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DefaultDbContext context) : base(context)
    {
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await DbSet.AddAsync(user, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await DbSet.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await DbSet.FirstOrDefaultAsync(u => string.Equals(u.Email.ToLower(), email.ToLower()), cancellationToken);
    }

    public async Task<bool> ExistsEmailRegisteredAsync(string email, CancellationToken cancellationToken)
    {
        return await DbSet.AnyAsync(u => string.Equals(u.Email.ToLower(), email.ToLower()), cancellationToken);
    }
}