using Application.Shared.Models;
using Application.UseCases.Exercises.InsertExercise.Commands;

namespace Application.UseCases.Exercises.InsertExercise;

public interface IInsertExerciseUseCase
{
    Task<Output> ExecuteAsync(InsertExerciseCommand command, CancellationToken cancellationToken);
}