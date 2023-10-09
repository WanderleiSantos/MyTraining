using System.Linq.Expressions;
using AspNetCore.IQueryable.Extensions;
using AspNetCore.IQueryable.Extensions.Filter;
using AspNetCore.IQueryable.Extensions.Sort;
using Core.Entities;
using Core.Interfaces.Persistence;
using Core.Interfaces.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    private readonly DefaultDbContext _context;
    protected readonly DbSet<TEntity> DbSet;

    protected Repository(DefaultDbContext context)
    {
        _context = context;
        DbSet = context.Set<TEntity>();
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<IEnumerable<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        return await DbSet
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public IQueryable<TEntity> PrepareQuery(ICustomQueryable search, string? sort)
    {
        return DbSet
            .AsNoTracking()
            .AsQueryable()
            .Filter(search)
            .Sort(sort);
    }
}