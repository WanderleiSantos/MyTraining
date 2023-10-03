using Application.Shared.Models;
using Application.UseCases.Exercises.UpdateExercise.Commands;

namespace Application.UseCases.Exercises.UpdateExercise;

public interface IUpdateExerciseUseCase
{
    Task<Output> ExecuteAsync(UpdateExerciseCommand command, CancellationToken cancellationToken);
}