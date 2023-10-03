using Application.Shared.Models;
using Application.UseCases.Users.UpdateUser.Commands;

namespace Application.UseCases.Users.UpdateUser;

public interface IUpdateUserUseCase
{
    Task<Output> ExecuteAsync(UpdateUserCommand command, CancellationToken cancellationToken);
}