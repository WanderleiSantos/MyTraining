using FluentValidation;
using Microsoft.Extensions.Logging;
using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.Exercises.UpdateExercise.Commands;
using MyTraining.Core.Interfaces.Persistence.Repositories;
using MyTraining.Infrastructure.Persistence;

namespace MyTraining.Application.UseCases.Exercises.UpdateExercise;

public class UpdateExerciseUseCase : IUpdateExerciseUseCase
{
    private readonly ILogger<UpdateExerciseUseCase> _logger;
    private readonly IExerciseRepository _repository;
    private readonly IValidator<UpdateExerciseCommand> _validator;

    public UpdateExerciseUseCase(ILogger<UpdateExerciseUseCase> logger, IExerciseRepository repository, IValidator<UpdateExerciseCommand> validator)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Output> ExecuteAsync(UpdateExerciseCommand command, CancellationToken cancellationToken)
    {
        var output = new Output();
        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);

            output.AddValidationResult(validationResult);

            if (!output.IsValid)
                return output;

            _logger.LogInformation("{UseCase} - Updating Exercise; Name: {Name}", nameof(UpdateExerciseUseCase),
                command.Name);

            var exercise = await _repository.GetByIdAsync(command.Id, cancellationToken);
            exercise?.Update(command.Name, command.Link);
            
            await _repository.UnitOfWork.CommitAsync();
            
            _logger.LogInformation("{UseCase} - Updated Exercise; Name: {Name}", nameof(UpdateExerciseUseCase),
                command.Name);

            output.AddResult($"Exercise updated; Id: {exercise?.Id}; Name: {exercise?.Name}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{UseCase} - An unexpected error has occurred;",
                nameof(UpdateExerciseUseCase));

            output.AddErrorMessage($"An unexpected error occurred while update the exercise");
        }

        return output;
    }
}