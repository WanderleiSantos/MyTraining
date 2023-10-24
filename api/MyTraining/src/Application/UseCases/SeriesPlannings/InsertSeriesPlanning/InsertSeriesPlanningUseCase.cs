using Application.Shared.Models;
using Application.UseCases.SeriesPlannings.InsertSeriesPlanning.Commands;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using Core.Shared.Errors;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.SeriesPlannings.InsertSeriesPlanning;

public class InsertSeriesPlanningUseCase : IInsertSeriesPlanningUseCase
{
    private readonly ILogger<InsertSeriesPlanningUseCase> _logger;
    private readonly ISeriesPlanningRepository _repository;
    private readonly IValidator<InsertSeriesPlanningCommand> _validator;
    private readonly IExerciseRepository _exerciseRepository;

    public InsertSeriesPlanningUseCase(ILogger<InsertSeriesPlanningUseCase> logger,
        ISeriesPlanningRepository repository, IValidator<InsertSeriesPlanningCommand> validator, IExerciseRepository exerciseRepository)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
        _exerciseRepository = exerciseRepository;
    }

    public async Task<Output> ExecuteAsync(InsertSeriesPlanningCommand command, CancellationToken cancellationToken)
    {
        var output = new Output();
        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            output.AddValidationResult(validationResult);

            if (!output.IsValid)
                return output;

            _logger.LogInformation("{UseCase} - Inserting SeriesPlanning;", nameof(InsertSeriesPlanningUseCase));

            foreach (var commandItem in command.SeriesPlanningInputs)
            {
                var seriesPlanning = new SeriesPlanning(commandItem.Machine, commandItem.SeriesNumber, commandItem.Repetitions,
                    commandItem.Charge, commandItem.Interval, command.TrainingSheetSeriesId);

                foreach (var exerciseId in commandItem.ExercisesIds)
                {
                    var exercise =
                        await _exerciseRepository.GetByIdAsync(exerciseId, command.UserId, cancellationToken);
                    seriesPlanning.Exercises.Add(exercise);
                }

                await _repository.AddAsync(seriesPlanning, cancellationToken);
                
            }
            
            await _repository.UnitOfWork.CommitAsync();

            _logger.LogInformation("{UseCase} - Inserted SeriesPlanning;", nameof(InsertSeriesPlanningUseCase));

            output.AddResult(null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{UseCase} - An unexpected error has occurred.", nameof(InsertSeriesPlanningUseCase));

            output.AddError(Error.Unexpected());
        }

        return output;
    }
}