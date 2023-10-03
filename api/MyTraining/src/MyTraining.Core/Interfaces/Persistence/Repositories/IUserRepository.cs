using MyTraining.Core.Entities;

namespace MyTraining.Core.Interfaces.Persistence.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ExistsEmailRegisteredAsync(string email, CancellationToken cancellationToken);
}