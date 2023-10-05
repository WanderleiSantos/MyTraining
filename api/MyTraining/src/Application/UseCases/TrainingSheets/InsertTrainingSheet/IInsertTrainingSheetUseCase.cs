using Application.Shared.Models;
using Application.UseCases.TrainingSheets.InsertTrainingSheet.Commands;

namespace Application.UseCases.TrainingSheets.InsertTrainingSheet;

public interface IInsertTrainingSheetUseCase
{
    Task<Output> ExecuteAsync(InsertTrainingSheetCommand command, CancellationToken cancellationToken);
}