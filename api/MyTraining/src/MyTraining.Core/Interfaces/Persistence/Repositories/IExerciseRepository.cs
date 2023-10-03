using MyTraining.Core.Entities;
using MyTraining.Core.Interfaces.Pagination;

namespace MyTraining.Core.Interfaces.Persistence.Repositories;

public interface IExerciseRepository : IRepository<Exercise>
{
    Task AddAsync(Exercise exercise, CancellationToken cancellationToken);
    Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Exercise>> GetAllAsync(Guid idUser, CancellationToken cancellationToken);
    Task<IPaginated<Exercise>> GetAllAsync(Guid idUser, int pageNumber, int pageSize, CancellationToken cancellationToken);
}