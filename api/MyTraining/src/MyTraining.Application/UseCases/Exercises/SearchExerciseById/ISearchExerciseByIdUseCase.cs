using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.Exercises.SearchExerciseById.Commands;

namespace MyTraining.Application.UseCases.Exercises.SearchExerciseById;

public interface ISearchExerciseByIdUseCase
{
    Task<Output> ExecuteAsync(SearchExerciseByIdCommand command, CancellationToken cancellationToken);
}