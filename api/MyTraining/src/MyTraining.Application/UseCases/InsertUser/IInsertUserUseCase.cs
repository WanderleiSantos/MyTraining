using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.InsertUser.Commands;

namespace MyTraining.Application.UseCases.InsertUser;

public interface IInsertUserUseCase
{
    Task<Output> ExecuteAsync(InsertUserCommand command, CancellationToken cancellationToken);
}