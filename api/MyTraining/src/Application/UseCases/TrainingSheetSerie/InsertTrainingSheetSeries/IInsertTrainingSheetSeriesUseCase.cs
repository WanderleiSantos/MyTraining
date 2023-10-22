using Application.Shared.Models;
using Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries.Commands;

namespace Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries;

public interface IInsertTrainingSheetSeriesUseCase
{
    Task<Output> ExecuteAsync(InsertTrainingSheetSeriesCommand command, CancellationToken cancellationToken);
}