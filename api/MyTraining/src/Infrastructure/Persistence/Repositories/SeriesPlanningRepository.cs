using Core.Entities;
using Core.Interfaces.Persistence.Repositories;

namespace Infrastructure.Persistence.Repositories;

public class SeriesPlanningRepository : Repository<SeriesPlanning>, ISeriesPlanningRepository
{
    public SeriesPlanningRepository(DefaultDbContext context) : base(context)
    {
    }

    public async Task AddAsync(SeriesPlanning seriesPlanning, CancellationToken cancellationToken)
    {
        await DbSet.AddAsync(seriesPlanning, cancellationToken);
    }
}