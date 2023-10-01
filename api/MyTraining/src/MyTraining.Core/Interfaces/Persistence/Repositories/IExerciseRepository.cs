using MyTraining.Core.Entities;

namespace MyTraining.Core.Interfaces.Persistence.Repositories;

public interface IExerciseRepository
{
    Task AddAsync(Exercise exercise, CancellationToken cancellationToken);
    Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Exercise>> GetByUserAsync(Guid idUser, CancellationToken cancellationToken);
}