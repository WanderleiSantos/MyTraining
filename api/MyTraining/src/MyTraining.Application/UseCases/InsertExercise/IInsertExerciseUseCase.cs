using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.InsertExercise.Commands;

namespace MyTraining.Application.UseCases.InsertExercise;

public interface IInsertExerciseUseCase
{
    Task<Output> ExecuteAsync(InsertExerciseCommand command, CancellationToken cancellationToken);
}