using MyTraining.Core.Entities;

namespace MyTraining.Core.Interfaces.Persistence.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}