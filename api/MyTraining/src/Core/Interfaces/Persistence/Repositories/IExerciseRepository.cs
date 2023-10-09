using Core.Entities;
using Core.Interfaces.Pagination;

namespace Core.Interfaces.Persistence.Repositories;

public interface IExerciseRepository : IRepository<Exercise>
{
    Task AddAsync(Exercise exercise, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<Exercise> exercises, CancellationToken cancellationToken);
    Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Exercise>> GetAllAsync(Guid idUser, CancellationToken cancellationToken);
    Task<IPaginated<Exercise>> GetAllAsync(Guid idUser, int pageNumber, int pageSize, CancellationToken cancellationToken);
}