using Application.Shared.Models;
using Application.UseCases.Users.SearchUserById.Commands;

namespace Application.UseCases.Users.SearchUserById;

public interface ISearchUserByIdUseCase
{
    Task<Output> ExecuteAsync(SearchUserByIdCommand command, CancellationToken cancellationToken);
}