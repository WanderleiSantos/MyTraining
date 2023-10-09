using System.Linq.Expressions;
using AspNetCore.IQueryable.Extensions;
using Core.Entities;

namespace Core.Interfaces.Persistence.Repositories;

public interface IRepository<TEntity>
{
    IUnitOfWork UnitOfWork { get; }
    
    Task<IEnumerable<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
    IQueryable<TEntity> PrepareQuery(ICustomQueryable search, string? sort);
}