using Microsoft.EntityFrameworkCore;
using MyTraining.Core.Entities;
using MyTraining.Core.Interfaces.Persistence;
using MyTraining.Core.Interfaces.Persistence.Repositories;

namespace MyTraining.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DefaultDbContext _context;
    private readonly DbSet<User> _dbSet;

    public UserRepository(DefaultDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<User>();
    }
    
    public IUnitOfWork UnitOfWork => _context;

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(user, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbSet.FirstOrDefaultAsync(u => string.Equals(u.Email, email, StringComparison.CurrentCultureIgnoreCase), cancellationToken);
    }

    public async Task<bool> ExistsEmailRegisteredAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }
}