using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.Exercises.SearchAllExercises.Commands;

namespace MyTraining.Application.UseCases.Exercises.SearchAllExercises;

public interface ISearchAllExercisesUseCase
{
    Task<Output> ExecuteAsync(SearchAllExercisesCommand command, CancellationToken cancellationToken);
}