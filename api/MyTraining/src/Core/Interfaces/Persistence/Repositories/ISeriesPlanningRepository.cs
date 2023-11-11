using Core.Entities;

namespace Core.Interfaces.Persistence.Repositories;

public interface ISeriesPlanningRepository : IRepository<SeriesPlanning>
{
    Task AddAsync(SeriesPlanning seriesPlanning, CancellationToken cancellationToken);
    Task<SeriesPlanning?> GetById(Guid id, CancellationToken cancellationToken);
}