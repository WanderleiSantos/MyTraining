namespace Application.UseCases.TrainingSheets.Services;

public interface IDeactivateTrainingSheetService
{
    Task Deactivate(Guid userId, CancellationToken cancellationToken);
}