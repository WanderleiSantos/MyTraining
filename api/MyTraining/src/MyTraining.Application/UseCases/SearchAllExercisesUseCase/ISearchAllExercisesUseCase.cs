using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.SearchAllExercisesUseCase.Commands;

namespace MyTraining.Application.UseCases.SearchAllExercisesUseCase;

public interface ISearchAllExercisesUseCase
{
    Task<Output> ExecuteAsync(SearchAllExercisesCommand command, CancellationToken cancellationToken);
}