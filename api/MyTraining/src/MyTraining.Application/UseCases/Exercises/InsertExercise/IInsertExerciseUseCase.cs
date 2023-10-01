using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.Exercises.InsertExercise.Commands;

namespace MyTraining.Application.UseCases.Exercises.InsertExercise;

public interface IInsertExerciseUseCase
{
    Task<Output> ExecuteAsync(InsertExerciseCommand command, CancellationToken cancellationToken);
}