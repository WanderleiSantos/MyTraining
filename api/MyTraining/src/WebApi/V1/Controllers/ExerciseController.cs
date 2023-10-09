using Application.UseCases.Exercises.InsertExercise;
using Application.UseCases.Exercises.SearchAllExercises;
using Application.UseCases.Exercises.SearchExerciseById;
using Application.UseCases.Exercises.SearchExerciseById.Commands;
using Application.UseCases.Exercises.UpdateExercise;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;
using WebApi.V1.Mappers;
using WebApi.V1.Models;
using WebApi.V1.Queries;

namespace WebApi.V1.Controllers;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ExerciseController : MainController
{
    private readonly ILogger<ExerciseController> _logger;
    private readonly IInsertExerciseUseCase _insertExerciseUseCase;
    private readonly ISearchExerciseByIdUseCase _searchExerciseByIdUseCase;
    private readonly ISearchAllExercisesUseCase _searchAllExercisesUseCase;
    private readonly IUpdateExerciseUseCase _updateExerciseUseCase;

    public ExerciseController(ICurrentUserService currentUserService, ILogger<ExerciseController> logger,
        IInsertExerciseUseCase insertExerciseUseCase, ISearchExerciseByIdUseCase searchExerciseByIdUseCase, ISearchAllExercisesUseCase searchAllExercisesUseCase, IUpdateExerciseUseCase updateExerciseUseCase) : base(currentUserService)
    {
        _logger = logger;
        _insertExerciseUseCase = insertExerciseUseCase;
        _searchExerciseByIdUseCase = searchExerciseByIdUseCase;
        _searchAllExercisesUseCase = searchAllExercisesUseCase;
        _updateExerciseUseCase = updateExerciseUseCase;
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
            var output = await _insertExerciseUseCase.ExecuteAsync(input.MapToApplication(this.CurrentUserService.UserId),
                cancellationToken);

            return CustomResponse(output);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred.");
            return BadRequest();
        }
    }

    [HttpPatch("{id:guid}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExerciseInput input, CancellationToken cancellationToken)
    {
        try
        {
            var output = await _updateExerciseUseCase.ExecuteAsync(input.MapToApplication(id), cancellationToken);
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
    public async Task<IActionResult> GetAll(
        [FromQuery] SearchAllExercises search,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await _searchAllExercisesUseCase.ExecuteAsync(search.MapToApplication(CurrentUserService.UserId), cancellationToken);
            return CustomResponse(output);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred");
            return BadRequest();
        }
    }
}