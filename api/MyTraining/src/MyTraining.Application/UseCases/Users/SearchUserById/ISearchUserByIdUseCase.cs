using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.Users.SearchUserById.Commands;

namespace MyTraining.Application.UseCases.Users.SearchUserById;

public interface ISearchUserByIdUseCase
{
    Task<Output> ExecuteAsync(SearchUserByIdCommand command, CancellationToken cancellationToken);
}