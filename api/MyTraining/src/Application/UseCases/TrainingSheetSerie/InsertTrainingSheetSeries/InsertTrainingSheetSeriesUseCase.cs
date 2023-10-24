using Application.Shared.Mappers;
using Application.Shared.Models;
using Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries.Commands;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using Core.Shared.Errors;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries;

public class InsertTrainingSheetSeriesUseCase : IInsertTrainingSheetSeriesUseCase
{
    private readonly ILogger<InsertTrainingSheetSeriesUseCase> _logger;
    private readonly ITrainingSheetSeriesRepository _repository;
    private readonly IValidator<InsertTrainingSheetSeriesCommand> _validator;

    public InsertTrainingSheetSeriesUseCase(ILogger<InsertTrainingSheetSeriesUseCase> logger,
        ITrainingSheetSeriesRepository repository, IValidator<InsertTrainingSheetSeriesCommand> validator)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Output> ExecuteAsync(InsertTrainingSheetSeriesCommand command,
        CancellationToken cancellationToken)
    {
        var output = new Output();
        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            output.AddValidationResult(validationResult);

            if (!output.IsValid)
                return output;

            _logger.LogInformation("{UseCase} - Insert TrainingSheetSeries; Name: {Name}",
                typeof(InsertTrainingSheetSeriesUseCase), command.Name);

            var trainingSheetSeries = new TrainingSheetSeries(command.Name, command.TrainingSheetId);
            await _repository.AddAsync(trainingSheetSeries, cancellationToken);
            await _repository.UnitOfWork.CommitAsync();
            
            _logger.LogInformation("{UseCase} - Inserted TrainingSheetSeries; Name: {Name}",
                nameof(InsertTrainingSheetSeriesUseCase), command.Name);
            
            output.AddResult(trainingSheetSeries.MapToResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{UseCase} - An unexpected error has occurred",
                nameof(InsertTrainingSheetSeriesUseCase));
            output.AddError(Error.Unexpected());
        }

        return output;
    }
}