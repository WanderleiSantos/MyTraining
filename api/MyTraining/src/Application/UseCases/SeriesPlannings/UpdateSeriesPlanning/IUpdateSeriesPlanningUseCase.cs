using Application.Shared.Models;
using Application.UseCases.SeriesPlannings.UpdateSeriesPlanning.Commands;

namespace Application.UseCases.SeriesPlannings.UpdateSeriesPlanning;

public interface IUpdateSeriesPlanningUseCase
{
    Task<Output> ExecuteAsync(UpdateSeriesPlanningCommand command, CancellationToken cancellationToken);
}