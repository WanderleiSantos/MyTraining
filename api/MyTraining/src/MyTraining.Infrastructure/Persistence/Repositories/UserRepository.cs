using Microsoft.EntityFrameworkCore;
using MyTraining.Core.Entities;
using MyTraining.Core.Interfaces.Persistence.Repositories;

namespace MyTraining.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DefaultDbContext _context;

    public UserRepository(DefaultDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsEmailRegisteredAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }
}