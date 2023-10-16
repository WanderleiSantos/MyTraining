using Application.Shared.Models;
using Application.UseCases.TrainingSheets.InsertTrainingSheet.Commands;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Application.Shared.Extensions;
using Application.Shared.Mappers;
using Application.UseCases.TrainingSheets.Services;
using Core.Shared.Errors;

namespace Application.UseCases.TrainingSheets.InsertTrainingSheet;

public class InsertTrainingSheetUseCase: IInsertTrainingSheetUseCase
{
    private readonly ILogger<InsertTrainingSheetUseCase> _logger;
    private readonly ITrainingSheetRepository _repository;
    private readonly IValidator<InsertTrainingSheetCommand> _validator;
    private readonly IDeactivateTrainingSheetService _deactivateTrainingSheetService;

    public InsertTrainingSheetUseCase(ILogger<InsertTrainingSheetUseCase> logger, ITrainingSheetRepository repository,
        IValidator<InsertTrainingSheetCommand> validator, IDeactivateTrainingSheetService deactivateTrainingSheetService)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
        _deactivateTrainingSheetService = deactivateTrainingSheetService;
    }

    public async Task<Output> ExecuteAsync(InsertTrainingSheetCommand command, CancellationToken cancellationToken)
    {
        var output = new Output();
        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            output.AddValidationResult(validationResult);

            if (!output.IsValid)
                return output;

            _logger.LogInformation("{UseCase} - Insert TrainingSheet; Name: {Name}", nameof(InsertTrainingSheetUseCase),
                command.Name);

            await _deactivateTrainingSheetService.Deactivate(command.UserId, cancellationToken);
            
            var trainingSheet = new TrainingSheet(command.Name, command.TimeExchange, command.UserId);

            await _repository.AddAsync(trainingSheet, cancellationToken);
            await _repository.UnitOfWork.CommitAsync();

            _logger.LogInformation("{UseCase} - Inserted TrainingSheet; Name: {Name}",
                nameof(InsertTrainingSheetUseCase), command.Name);
            
            output.AddResult(trainingSheet.MapToResponse());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{UseCase} - An unexpected error has occurred", nameof(InsertTrainingSheetUseCase));

            output.AddError(Error.Unexpected());
        }

        return output;
    }
}