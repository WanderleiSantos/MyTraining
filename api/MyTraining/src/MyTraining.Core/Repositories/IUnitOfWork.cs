namespace MyTraining.Core.Repositories;

public interface IUnitOfWork
{
    Task<bool> CommitAsync();
    Task RollbackAsync();
}