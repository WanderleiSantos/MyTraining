using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.Exercises.UpdateExercise.Commands;

namespace MyTraining.Application.UseCases.Exercises.UpdateExercise;

public interface IUpdateExerciseUseCase
{
    Task<Output> ExecuteAsync(UpdateExerciseCommand command, CancellationToken cancellationToken);
}