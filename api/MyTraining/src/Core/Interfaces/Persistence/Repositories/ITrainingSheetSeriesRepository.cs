using Core.Entities;

namespace Core.Interfaces.Persistence.Repositories;

public interface ITrainingSheetSeriesRepository : IRepository<TrainingSheetSeries>
{
    Task AddAsync(TrainingSheetSeries trainingSheetSeries, CancellationToken cancellationToken);
}