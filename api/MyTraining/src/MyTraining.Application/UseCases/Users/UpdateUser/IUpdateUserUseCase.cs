using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.Users.UpdateUser.Commands;

namespace MyTraining.Application.UseCases.Users.UpdateUser;

public interface IUpdateUserUseCase
{
    Task<Output> ExecuteAsync(UpdateUserCommand command, CancellationToken cancellationToken);
}