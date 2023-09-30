using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTraining.API.Controllers;
using MyTraining.API.V1.Mappers;
using MyTraining.API.V1.Models;
using MyTraining.Application.UseCases.InsertUser;
using MyTraining.Core.Interfaces;
using MyTraining.Core.Interfaces.Extensions;

namespace MyTraining.API.V1.Controllers;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UserController : MainController
{
    private readonly ILogger<UserController> _logger;
    private readonly IInsertUserUseCase _insertUserUseCase;

    public UserController(ICurrentUser currentUser, ILogger<UserController> logger,
        IInsertUserUseCase insertUserUseCase) : base(currentUser)
    {
        _logger = logger;
        _insertUserUseCase = insertUserUseCase;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create(
        [FromBody] InsertUserInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await _insertUserUseCase.ExecuteAsync(input.MapToApplication(), cancellationToken);
            
            return CustomResponse(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            return BadRequest();
        }
    }
}