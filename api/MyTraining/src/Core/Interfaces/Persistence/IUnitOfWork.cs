namespace Core.Interfaces.Persistence;

public interface IUnitOfWork
{
    Task<bool> CommitAsync();
    Task RollbackAsync();
}