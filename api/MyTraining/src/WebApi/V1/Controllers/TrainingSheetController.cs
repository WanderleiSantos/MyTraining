using Application.Shared.Authentication;
using Application.UseCases.TrainingSheets.InsertTrainingSheet;
using Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;
using WebApi.Shared;
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
    private readonly IInsertTrainingSheetSeriesUseCase _insertTrainingSheetSeriesUseCase;

    public TrainingSheetController(ICurrentUserService currentUserService, ILogger<TrainingSheetController> logger,
        IInsertTrainingSheetUseCase insertTrainingSheetUseCase, IInsertTrainingSheetSeriesUseCase insertTrainingSheetSeriesUseCase) : base(currentUserService)
    {
        _logger = logger;
        _insertTrainingSheetUseCase = insertTrainingSheetUseCase;
        _insertTrainingSheetSeriesUseCase = insertTrainingSheetSeriesUseCase;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Create([FromBody] InsertTrainingSheetInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            var output =
                await _insertTrainingSheetUseCase.ExecuteAsync(input.MapToApplication(this.CurrentUser.UserId),
                    cancellationToken);

            return CustomResponse(output);
        }
        catch (Exception e)
        {
            _logger.LogError(e, Constants.UnexpectedErrorDescription);
            return InternalServerError(Constants.UnexpectedErrorDescription);
        }
    }
    
    [HttpPost("{id:guid}/serie")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateSerie(Guid id,[FromBody] InsertTrainingSheetSeriesInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            var output =
                await _insertTrainingSheetSeriesUseCase.ExecuteAsync(input.MapToApplication(this.CurrentUser.UserId),
                    cancellationToken);

            return CustomResponse(output);
        }
        catch (Exception e)
        {
            _logger.LogError(e, Constants.UnexpectedErrorDescription);
            return InternalServerError(Constants.UnexpectedErrorDescription);
        }
    }
    
    
}