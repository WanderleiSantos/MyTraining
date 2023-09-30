using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.SignIn.Commands;

namespace MyTraining.Application.UseCases.SignIn;

public interface ISignInUseCase
{
    Task<Output> ExecuteAsync(SignInCommand command, CancellationToken cancellationToken);
}