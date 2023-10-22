using Core.Entities;
using Core.Interfaces.Persistence.Repositories;

namespace Infrastructure.Persistence.Repositories;

public class TrainingSheetSeriesRepository : Repository<TrainingSheetSeries>, ITrainingSheetSeriesRepository
{
    public TrainingSheetSeriesRepository(DefaultDbContext context) : base(context)
    {
    }

    public async Task AddAsync(TrainingSheetSeries trainingSheetSeries, CancellationToken cancellationToken)
    {
        await DbSet.AddAsync(trainingSheetSeries, cancellationToken);
    }
}