using Application.Shared.Models;
using Application.UseCases.Exercises.SearchAllExercises.Commands;
using Application.UseCases.Exercises.SearchAllExercises.Responses;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Application.Shared.Mappers;
using Core.Interfaces.Persistence.Repositories;
using Core.Shared.Errors;

namespace Application.UseCases.Exercises.SearchAllExercises;

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

            var result = await _repository.GetAllAsync(
                command.UserId,
                command,
                command.PageNumber, 
                command.PageSize, 
                cancellationToken);

            _logger.LogInformation("{UseCase} - Search Exercises by user finish successfully, userId: {id}",
                nameof(SearchAllExercisesUseCase), command.UserId);

            output.AddResult(new PaginatedOutput<SearchAllExercisesResponse>(result, 
                result.Items.MapToResponse()));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{UseCase} - An unexpected error has occurred; ", nameof(SearchAllExercisesUseCase));
            output.AddError(Error.Unexpected());
        }

        return output;
    }
}