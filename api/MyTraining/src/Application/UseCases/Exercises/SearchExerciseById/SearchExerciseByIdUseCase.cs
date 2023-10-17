using Application.Shared.Models;
using Application.UseCases.Exercises.SearchExerciseById.Commands;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Application.Shared.Mappers;
using Core.Interfaces.Persistence.Repositories;
using Core.Shared.Errors;

namespace Application.UseCases.Exercises.SearchExerciseById;

public class SearchExerciseByIdUseCase : ISearchExerciseByIdUseCase
{
    private readonly ILogger<SearchExerciseByIdUseCase> _logger;
    private readonly IExerciseRepository _repository;
    private readonly IValidator<SearchExerciseByIdCommand> _validator;

    public SearchExerciseByIdUseCase(ILogger<SearchExerciseByIdUseCase> logger, IExerciseRepository repository,
        IValidator<SearchExerciseByIdCommand> validator)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Output> ExecuteAsync(SearchExerciseByIdCommand command, CancellationToken cancellationToken)
    {
        var output = new Output();
        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            output.AddValidationResult(validationResult);

            if (!output.IsValid)
                return output;

            _logger.LogInformation("{UseCase} - Search exercise by id: {id}", nameof(SearchExerciseByIdUseCase),
                command.Id);

            var result = await _repository.GetByIdAsync(command.Id, cancellationToken);

            _logger.LogInformation("{UseCase} - Search Exercise finish successfully, id: {id}",
                nameof(SearchExerciseByIdUseCase), command.Id);

            output.AddResult(result?.MapToResponse());
        }
        catch (Exception e)
        {
            _logger.LogError("{UseCase} -  An unexpected error has occurred;", nameof(SearchExerciseByIdUseCase));
            output.AddError(Error.Unexpected());
        }

        return output;
    }
}