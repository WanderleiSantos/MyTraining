using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.SearchExercisesByUserUseCase.Commands;

namespace MyTraining.Application.UseCases.SearchExercisesByUserUseCase;

public interface ISearchExerciseByUserUseCase
{
    Task<Output> ExecuteAsync(SearchExerciseByUserCommand command, CancellationToken cancellationToken);
}