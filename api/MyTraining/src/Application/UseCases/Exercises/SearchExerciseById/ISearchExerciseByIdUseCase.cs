using Application.Shared.Models;
using Application.UseCases.Exercises.SearchExerciseById.Commands;

namespace Application.UseCases.Exercises.SearchExerciseById;

public interface ISearchExerciseByIdUseCase
{
    Task<Output> ExecuteAsync(SearchExerciseByIdCommand command, CancellationToken cancellationToken);
}