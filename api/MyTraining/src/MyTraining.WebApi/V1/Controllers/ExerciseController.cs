using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTraining.API.Controllers;
using MyTraining.API.V1.Mappers;
using MyTraining.API.V1.Models;
using MyTraining.Application.UseCases.InsertExercise;
using MyTraining.Core.Interfaces.Extensions;

namespace MyTraining.API.V1.Controllers;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ExerciseController : MainController
{
    private readonly ILogger<ExerciseController> _logger;
    private readonly IInsertExerciseUseCase _insertExerciseUseCase;

    public ExerciseController(ICurrentUser currentUser, ILogger<ExerciseController> logger,
        IInsertExerciseUseCase insertExerciseUseCase) : base(currentUser)
    {
        _logger = logger;
        _insertExerciseUseCase = insertExerciseUseCase;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create(
        [FromBody] InsertExerciseInput input,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var output = await _insertExerciseUseCase.ExecuteAsync(input.MapToApplication(), cancellationToken);
            if (output.IsValid)
                return Ok(output.Result);

            return BadRequest(output);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred.");
            return BadRequest();
        }
    }
}