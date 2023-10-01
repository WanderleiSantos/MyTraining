using FluentValidation;
using Microsoft.Extensions.Logging;
using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.SearchExercisesByUserUseCase.Commands;
using MyTraining.Core.Interfaces.Persistence.Repositories;

namespace MyTraining.Application.UseCases.SearchExercisesByUserUseCase;

public class SearchExerciseByUserUseCase : ISearchExerciseByUserUseCase
{
    private readonly ILogger<SearchExerciseByUserUseCase> _logger;
    private readonly IExerciseRepository _repository;
    private readonly IValidator<SearchExerciseByUserCommand> _validator;

    public SearchExerciseByUserUseCase(ILogger<SearchExerciseByUserUseCase> logger, IExerciseRepository repository,
        IValidator<SearchExerciseByUserCommand> validator)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Output> ExecuteAsync(SearchExerciseByUserCommand command, CancellationToken cancellationToken)
    {
        var output = new Output();

        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            output.AddValidationResult(validationResult);
            if (!output.IsValid)
                return output;

            _logger.LogInformation("{UseCase} - Search exercise by userId: {id}", nameof(SearchExerciseByUserUseCase),
                command.UserId);

            var result = await _repository.GetByUserAsync(command.UserId, cancellationToken);

            _logger.LogInformation("{UseCase} - Search Exercises by user finish successfully, userId: {id}",
                nameof(SearchExerciseByUserUseCase), command.UserId);

            output.AddResult(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{UseCase} - An unexpected error has occurred; ", nameof(SearchExerciseByUserUseCase));
            output.AddErrorMessage("An unexpected error occurred while searching the exercises.");
        }

        return output;
    }
}