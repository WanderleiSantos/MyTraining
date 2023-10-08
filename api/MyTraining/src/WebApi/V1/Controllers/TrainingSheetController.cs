using Application.UseCases.TrainingSheets.InsertTrainingSheet;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;
using WebApi.V1.Mappers;
using WebApi.V1.Models;

namespace WebApi.V1.Controllers;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class TrainingSheetController : MainController
{
    private readonly ILogger<TrainingSheetController> _logger;
    private readonly IInsertTrainingSheetUseCase _insertTrainingSheetUseCase;

    public TrainingSheetController(ICurrentUserService currentUserService, ILogger<TrainingSheetController> logger,
        IInsertTrainingSheetUseCase insertTrainingSheetUseCase) : base(currentUserService)
    {
        _logger = logger;
        _insertTrainingSheetUseCase = insertTrainingSheetUseCase;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Create([FromBody] InsertTrainingSheetInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            var output =
                await _insertTrainingSheetUseCase.ExecuteAsync(input.MapToApplication(this.CurrentUserService.UserId),
                    cancellationToken);

            return CustomResponse(output);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred.");
            return BadRequest();
        }
    }
}