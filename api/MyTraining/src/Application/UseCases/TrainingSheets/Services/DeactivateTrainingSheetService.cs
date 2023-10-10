using Core.Interfaces.Persistence.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.TrainingSheets.Services;

public class DeactivateTrainingSheetService : IDeactivateTrainingSheetService
{

    private readonly ILogger<DeactivateTrainingSheetService> _logger;
    private readonly ITrainingSheetRepository _repository;

    public DeactivateTrainingSheetService(ILogger<DeactivateTrainingSheetService> logger, ITrainingSheetRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task Deactivate(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("{Service} - Deactivating TrainingSheet", nameof(DeactivateTrainingSheetService));
            
            var trainingSheet = await _repository.GetActive(userId, cancellationToken);
            trainingSheet?.Deactivate();
            
            _logger.LogInformation("{Service} - Deactivate TrainingSheet; Name: {Name}", nameof(DeactivateTrainingSheetService),
                trainingSheet?.Name);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{Service} - An unexpected error has occurred", nameof(DeactivateTrainingSheetService));
        }
    }
}