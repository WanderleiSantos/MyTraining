using Core.Entities;
using Core.Interfaces.Persistence;
using Core.Interfaces.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class TrainingSheetRepository : ITrainingSheetRepository
{
    private readonly DefaultDbContext _context;
    private readonly DbSet<TrainingSheet> _dbSet;

    public TrainingSheetRepository(DefaultDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TrainingSheet>();
    }

    public IUnitOfWork UnitOfWork => _context;
    
    public async Task AddAsync(TrainingSheet trainingSheet, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(trainingSheet, cancellationToken);
    }
}