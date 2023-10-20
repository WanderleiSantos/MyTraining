using AspNetCore.IQueryable.Extensions.Sort;
using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Interfaces.Pagination;
using Core.Interfaces.Persistence.Repositories;
using Infrastructure.Extensions;

namespace Infrastructure.Persistence.Repositories;

public class ExerciseRepository : Repository<Exercise>, IExerciseRepository
{
    public ExerciseRepository(DefaultDbContext context) : base(context)
    {
    }

    public async Task AddAsync(Exercise exercise, CancellationToken cancellationToken)
    {
        await DbSet.AddAsync(exercise, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Exercise> exercises, CancellationToken cancellationToken)
    {
        await DbSet.AddRangeAsync(exercises, cancellationToken);
    }

    public async Task<Exercise?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        return await DbSet.SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
    }

    public async Task<IPaginated<Exercise>> GetAllAsync(Guid idUser, IQuerySort search, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        return await PrepareQuery(search, search.Sort)
            .Where(x => x.UserId == idUser)
            .ToPaginatedAsync(pageNumber, pageSize, cancellationToken);
    }
}