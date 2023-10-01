using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTraining.API.Controllers;
using MyTraining.API.V1.Mappers;
using MyTraining.API.V1.Models;
using MyTraining.Application.UseCases.InsertExercise;
using MyTraining.Application.UseCases.SearchAllExercisesUseCase;
using MyTraining.Application.UseCases.SearchAllExercisesUseCase.Commands;
using MyTraining.Application.UseCases.SearchExerciseById;
using MyTraining.Application.UseCases.SearchExerciseById.Commands;
using MyTraining.Core.Interfaces.Extensions;

namespace MyTraining.API.V1.Controllers;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ExerciseController : MainController
{
    private readonly ILogger<ExerciseController> _logger;
    private readonly IInsertExerciseUseCase _insertExerciseUseCase;
    private readonly ISearchExerciseByIdUseCase _searchExerciseByIdUseCase;
    private readonly ISearchAllExercisesUseCase _searchAllExercisesUseCase;

    public ExerciseController(ICurrentUser currentUser, ILogger<ExerciseController> logger,
        IInsertExerciseUseCase insertExerciseUseCase, ISearchExerciseByIdUseCase searchExerciseByIdUseCase, ISearchAllExercisesUseCase searchAllExercisesUseCase) : base(currentUser)
    {
        _logger = logger;
        _insertExerciseUseCase = insertExerciseUseCase;
        _searchExerciseByIdUseCase = searchExerciseByIdUseCase;
        _searchAllExercisesUseCase = searchAllExercisesUseCase;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Create(
        [FromBody] InsertExerciseInput input,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var output = await _insertExerciseUseCase.ExecuteAsync(input.MapToApplication(this.CurrentUser.UserId),
                cancellationToken);

            return CustomResponse(output);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred.");
            return BadRequest();
        }
    }

    [HttpGet("{id:guid}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new SearchExerciseByIdCommand() { Id = id };
            var output = await _searchExerciseByIdUseCase.ExecuteAsync(command, cancellationToken);

            return CustomResponse(output);
        }
        catch (Exception e)
        { 
            _logger.LogError(e, "An unexpected error occurred");
            return BadRequest();
        }
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var command = new SearchAllExercisesCommand() { UserId = this.CurrentUser.UserId };
            var output = await _searchAllExercisesUseCase.ExecuteAsync(command, cancellationToken);
            return CustomResponse(output);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred");
            return BadRequest();
        }
    }
}