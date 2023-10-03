using Microsoft.EntityFrameworkCore;
using MyTraining.Core.Entities;
using MyTraining.Core.Interfaces.Pagination;
using MyTraining.Core.Interfaces.Persistence;
using MyTraining.Core.Interfaces.Persistence.Repositories;
using MyTraining.Infrastructure.Extensions;

namespace MyTraining.Infrastructure.Persistence.Repositories;

public class ExerciseRepository : IExerciseRepository
{
    private readonly DefaultDbContext _context;
    private readonly DbSet<Exercise> _dbSet;

    public ExerciseRepository(DefaultDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<Exercise>();
    }
    
    public IUnitOfWork UnitOfWork => _context;

    public async Task AddAsync(Exercise exercise, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(exercise, cancellationToken);
    }

    public async Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
    
    public async Task<IEnumerable<Exercise>> GetAllAsync(Guid idUser, CancellationToken cancellationToken)
    {
        return await _dbSet.AsTracking().Where(x => x.IdUser == idUser).ToListAsync(cancellationToken);
    }

    public async Task<IPaginated<Exercise>> GetAllAsync(Guid idUser, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsTracking()
            .Where(x => x.IdUser == idUser)
            .ToPaginatedAsync(pageNumber, pageSize);
    }
}