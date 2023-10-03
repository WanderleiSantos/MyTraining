using Application.Shared.Models;
using Application.UseCases.Auth.SignIn.Commands;

namespace Application.UseCases.Auth.SignIn;

public interface ISignInUseCase
{
    Task<Output> ExecuteAsync(SignInCommand command, CancellationToken cancellationToken);
}