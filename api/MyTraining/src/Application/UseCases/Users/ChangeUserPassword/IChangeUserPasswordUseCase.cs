using Application.Shared.Models;
using Application.UseCases.Users.ChangeUserPassword.Commands;

namespace Application.UseCases.Users.ChangeUserPassword;

public interface IChangeUserPasswordUseCase
{
    Task<Output> ExecuteAsync(ChangeUserPasswordCommand command, CancellationToken cancellationToken);
}