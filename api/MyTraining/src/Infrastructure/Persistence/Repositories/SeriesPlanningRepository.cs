using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

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

    public async Task<SeriesPlanning?> GetById(Guid id, CancellationToken cancellationToken)
    {
        return await DbSet.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}