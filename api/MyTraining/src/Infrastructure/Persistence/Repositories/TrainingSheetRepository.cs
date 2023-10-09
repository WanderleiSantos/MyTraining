using Core.Entities;
using Core.Interfaces.Persistence.Repositories;

namespace Infrastructure.Persistence.Repositories;

public class TrainingSheetRepository : Repository<TrainingSheet>, ITrainingSheetRepository
{
    public TrainingSheetRepository(DefaultDbContext context) : base(context)
    {
    }

    public async Task AddAsync(TrainingSheet trainingSheet, CancellationToken cancellationToken)
    {
        await DbSet.AddAsync(trainingSheet, cancellationToken);
    }
}