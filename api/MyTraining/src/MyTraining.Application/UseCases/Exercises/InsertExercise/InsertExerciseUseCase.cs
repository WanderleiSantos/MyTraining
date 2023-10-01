using FluentValidation;
using Microsoft.Extensions.Logging;
using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.Exercises.InsertExercise.Commands;
using MyTraining.Core.Entities;
using MyTraining.Core.Interfaces.Persistence.Repositories;
using MyTraining.Infrastructure.Persistence;

namespace MyTraining.Application.UseCases.Exercises.InsertExercise;

public class InsertExerciseUseCase : IInsertExerciseUseCase
{
    private readonly ILogger<InsertExerciseUseCase> _logger;
    private readonly IExerciseRepository _repository;
    private readonly IValidator<InsertExerciseCommand> _validator;
    private readonly DefaultDbContext _context;

    public InsertExerciseUseCase(ILogger<InsertExerciseUseCase> logger, IExerciseRepository repository,
        IValidator<InsertExerciseCommand> validator, DefaultDbContext context)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
        _context = context;
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
            await _context.CommitAsync();

            _logger.LogInformation("{UseCase} - Inserted Exercise; Name: {Name}", nameof(InsertExerciseUseCase),
                command.Name);

            output.AddResult($"Exercise inserted; Id: {exercise.Id}; Name: {exercise.Name}");
            
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{UseCase} - An unexpected error has occurred;",
                nameof(InsertExerciseUseCase));

            output.AddErrorMessage($"An unexpected error occurred while inserting the exercise");
        }

        return output;
    }
}