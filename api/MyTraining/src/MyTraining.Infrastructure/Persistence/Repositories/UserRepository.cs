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

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
    }
}