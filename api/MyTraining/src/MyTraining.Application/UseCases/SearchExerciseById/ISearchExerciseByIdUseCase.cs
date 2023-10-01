using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.SearchExerciseById.Commands;

namespace MyTraining.Application.UseCases.SearchExerciseById;

public interface ISearchExerciseByIdUseCase
{
    Task<Output> ExecuteAsync(SearchExerciseByIdCommand command, CancellationToken cancellationToken);
}