using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.Auth.SignIn.Commands;

namespace MyTraining.Application.UseCases.Auth.SignIn;

public interface ISignInUseCase
{
    Task<Output> ExecuteAsync(SignInCommand command, CancellationToken cancellationToken);
}