using Application.Shared.Models;
using Application.UseCases.Users.InsertUser.Commands;

namespace Application.UseCases.Users.InsertUser;

public interface IInsertUserUseCase
{
    Task<Output> ExecuteAsync(InsertUserCommand command, CancellationToken cancellationToken);
}