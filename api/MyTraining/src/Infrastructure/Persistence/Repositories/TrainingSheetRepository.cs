using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

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

    public async Task<TrainingSheet?> GetActive(Guid userId, CancellationToken cancellationToken)
    {
        return await DbSet.SingleOrDefaultAsync(x => x.UserId == userId && x.Active, cancellationToken);
    }
}