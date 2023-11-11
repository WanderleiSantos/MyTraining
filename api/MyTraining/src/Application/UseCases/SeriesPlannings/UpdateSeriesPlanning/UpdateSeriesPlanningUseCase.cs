using Application.Shared.Models;
using Application.UseCases.SeriesPlannings.UpdateSeriesPlanning.Commands;
using Core.Interfaces.Persistence.Repositories;
using Core.Shared.Errors;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.SeriesPlannings.UpdateSeriesPlanning;

public class UpdateSeriesPlanningUseCase : IUpdateSeriesPlanningUseCase
{
    private readonly ILogger<UpdateSeriesPlanningUseCase> _logger;
    private readonly ISeriesPlanningRepository _repository;
    private readonly IValidator<UpdateSeriesPlanningCommand> _validator;

    public UpdateSeriesPlanningUseCase(ILogger<UpdateSeriesPlanningUseCase> logger,
        ISeriesPlanningRepository repository, IValidator<UpdateSeriesPlanningCommand> validator)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Output> ExecuteAsync(UpdateSeriesPlanningCommand command, CancellationToken cancellationToken)
    {
        var output = new Output();
        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            output.AddValidationResult(validationResult);
            if (!output.IsValid)
                return output;

            _logger.LogInformation("{UseCase} - Search SeriesPlanning by id: {id};",
                nameof(UpdateSeriesPlanningUseCase), command.Id);

            var seriePlanning = await _repository.GetById(command.Id, cancellationToken);

            if (seriePlanning is null)
            {
                _logger.LogWarning("{UseCase} - Search SeriesPlanning does not exist, id: {id};",
                    nameof(UpdateSeriesPlanningUseCase), command.Id);

                output.AddError(Errors.SeriesPlanning.DoesNotExist);
                return output;
            }

            _logger.LogInformation("{UseCase} - Updating SeriesPlanning by id: {id};",
                nameof(UpdateSeriesPlanningUseCase), command.Id);

            seriePlanning.Update(command.Machine, command.SeriesNumber, command.Repetitions, command.Charge,
                command.Interval);

            _logger.LogInformation("{UseCase} - SeriesPlanning updated successfully; Id: {id};",
                nameof(UpdateSeriesPlanningUseCase), command.Id);


            output.AddResult(null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{UseCase} - An unexpected error has occurred;",
                nameof(UpdateSeriesPlanningUseCase));
            output.AddError(Error.Unexpected());
        }

        return output;
    }
}