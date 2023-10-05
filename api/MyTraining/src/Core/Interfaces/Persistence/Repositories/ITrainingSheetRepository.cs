using Core.Entities;

namespace Core.Interfaces.Persistence.Repositories;

public interface ITrainingSheetRepository : IRepository<TrainingSheet>
{
    Task AddAsync(TrainingSheet trainingSheet, CancellationToken cancellationToken);
}