using Application.Shared.Models;
using Application.UseCases.Exercises.UpdateExercise.Commands;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Core.Interfaces.Persistence.Repositories;
using Core.Shared.Errors;
using Errors = Core.Shared.Errors.Errors;

namespace Application.UseCases.Exercises.UpdateExercise;

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

            var exercise = await _repository.GetByIdAsync(command.Id, command.UserId, cancellationToken);
            
            if (exercise is null)
            {
                output.AddError(Errors.Exercise.DoesNotExist);
                _logger.LogWarning("Exercise does not exist.");
                return output;
            }
            
            exercise?.Update(command.Name, command.Link);
            
            await _repository.UnitOfWork.CommitAsync();
            
            _logger.LogInformation("{UseCase} - Updated Exercise; Name: {Name}", nameof(UpdateExerciseUseCase),
                command.Name);

            output.AddResult(null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{UseCase} - An unexpected error has occurred;",
                nameof(UpdateExerciseUseCase));

            output.AddError(Error.Unexpected());
        }

        return output;
    }
}