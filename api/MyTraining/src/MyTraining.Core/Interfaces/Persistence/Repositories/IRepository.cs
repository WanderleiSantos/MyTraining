namespace MyTraining.Core.Interfaces.Persistence.Repositories;

public interface IRepository<T> 
{
    IUnitOfWork UnitOfWork { get; }
}