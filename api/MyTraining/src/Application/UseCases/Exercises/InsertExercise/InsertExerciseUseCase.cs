using Application.Shared.Mappers;
using Application.Shared.Models;
using Application.UseCases.Exercises.InsertExercise.Commands;
using Core.Common.Errors;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using Infrastructure.Persistence;

namespace Application.UseCases.Exercises.InsertExercise;

public class InsertExerciseUseCase : IInsertExerciseUseCase
{
    private readonly ILogger<InsertExerciseUseCase> _logger;
    private readonly IExerciseRepository _repository;
    private readonly IValidator<InsertExerciseCommand> _validator;

    public InsertExerciseUseCase(ILogger<InsertExerciseUseCase> logger, IExerciseRepository repository,
        IValidator<InsertExerciseCommand> validator)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Output> ExecuteAsync(InsertExerciseCommand command, CancellationToken cancellationToken)
    {
        var output = new Output();
        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            output.AddValidationResult(validationResult);

            if (!output.IsValid)
                return output;

            _logger.LogInformation("{UseCase} - Inserting Exercise; Name: {Name}", nameof(InsertExerciseUseCase),
                command.Name);

            var exercise = new Exercise(command.Name, command.Link, command.UserId);

            await _repository.AddAsync(exercise, cancellationToken);
            await _repository.UnitOfWork.CommitAsync();

            _logger.LogInformation("{UseCase} - Inserted Exercise; Name: {Name}", nameof(InsertExerciseUseCase),
                command.Name);

            output.AddResult(exercise.MapToResponseInsertExercise());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{UseCase} - An unexpected error has occurred.", nameof(InsertExerciseUseCase));

            output.AddError(Error.Unexpected());
        }

        return output;
    }
}