using Microsoft.EntityFrameworkCore;
using MyTraining.Core.Entities;
using MyTraining.Core.Interfaces.Persistence.Repositories;

namespace MyTraining.Infrastructure.Persistence.Repositories;

public class ExerciseRepository : IExerciseRepository
{
    private readonly DefaultDbContext _context;


    public ExerciseRepository(DefaultDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Exercise exercise, CancellationToken cancellationToken)
    {
        await _context.Exercises.AddAsync(exercise, cancellationToken);
    }

    public async Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Exercises.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}