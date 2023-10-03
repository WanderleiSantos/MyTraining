using Application.Shared.Models;
using Application.UseCases.Exercises.SearchAllExercises.Commands;

namespace Application.UseCases.Exercises.SearchAllExercises;

public interface ISearchAllExercisesUseCase
{
    Task<Output> ExecuteAsync(SearchAllExercisesCommand command, CancellationToken cancellationToken);
}