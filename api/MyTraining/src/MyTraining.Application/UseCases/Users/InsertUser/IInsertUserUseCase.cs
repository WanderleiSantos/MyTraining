using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.Users.InsertUser.Commands;

namespace MyTraining.Application.UseCases.Users.InsertUser;

public interface IInsertUserUseCase
{
    Task<Output> ExecuteAsync(InsertUserCommand command, CancellationToken cancellationToken);
}