using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.SearchUserById.Commands;

namespace MyTraining.Application.UseCases.SearchUserById;

public interface ISearchUserByIdUseCase
{
    Task<Output> ExecuteAsync(SearchUserByIdCommand command, CancellationToken cancellationToken);
}