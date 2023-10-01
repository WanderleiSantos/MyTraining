using FluentValidation;
using Microsoft.Extensions.Logging;
using MyTraining.Application.Shared.Mappers;
using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.Exercises.SearchAllExercises.Commands;
using MyTraining.Core.Interfaces.Persistence.Repositories;

namespace MyTraining.Application.UseCases.Exercises.SearchAllExercises;

public class SearchAllExercisesUseCase : ISearchAllExercisesUseCase
{
    private readonly ILogger<SearchAllExercisesUseCase> _logger;
    private readonly IExerciseRepository _repository;
    private readonly IValidator<SearchAllExercisesCommand> _validator;

    public SearchAllExercisesUseCase(ILogger<SearchAllExercisesUseCase> logger, IExerciseRepository repository,
        IValidator<SearchAllExercisesCommand> validator)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Output> ExecuteAsync(SearchAllExercisesCommand command, CancellationToken cancellationToken)
    {
        var output = new Output();

        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            output.AddValidationResult(validationResult);
            if (!output.IsValid)
                return output;

            _logger.LogInformation("{UseCase} - Search exercise by userId: {id}", nameof(SearchAllExercisesUseCase),
                command.UserId);

            var result = await _repository.GetAllAsync(command.UserId, cancellationToken);

            _logger.LogInformation("{UseCase} - Search Exercises by user finish successfully, userId: {id}",
                nameof(SearchAllExercisesUseCase), command.UserId);

            output.AddResult(result?.MapToApplication());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{UseCase} - An unexpected error has occurred; ", nameof(SearchAllExercisesUseCase));
            output.AddErrorMessage("An unexpected error occurred while searching the exercises.");
        }

        return output;
    }
}