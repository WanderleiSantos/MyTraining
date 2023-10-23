using Application.Shared.Models;
using Application.UseCases.SeriesPlannings.InsertSeriesPlanning.Commands;

namespace Application.UseCases.SeriesPlannings.InsertSeriesPlanning;

public interface IInsertSeriesPlanningUseCase
{
    Task<Output> ExecuteAsync(InsertSeriesPlanningCommand command, CancellationToken cancellationToken);
}