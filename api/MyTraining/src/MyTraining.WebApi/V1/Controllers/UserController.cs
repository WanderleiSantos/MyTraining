using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTraining.API.Controllers;
using MyTraining.API.V1.Mappers;
using MyTraining.API.V1.Models;
using MyTraining.Application.UseCases.InsertUser;
using MyTraining.Application.UseCases.SearchUserById;
using MyTraining.Application.UseCases.SearchUserById.Commands;
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
    private readonly ISearchUserByIdUseCase _searchUserByIdUseCase;

    public UserController(ICurrentUser currentUser, ILogger<UserController> logger, IInsertUserUseCase insertUserUseCase, ISearchUserByIdUseCase searchUserByIdUseCase) : base(currentUser)
    {
        _logger = logger;
        _insertUserUseCase = insertUserUseCase;
        _searchUserByIdUseCase = searchUserByIdUseCase;
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
    
    [HttpGet("{id:guid}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new SearchUserByIdCommand() { Id = id };
            var output = await _searchUserByIdUseCase.ExecuteAsync(command, cancellationToken);

            return CustomResponse(output);
        }
        catch (Exception e)
        { 
            _logger.LogError(e, "An unexpected error occurred");
            return BadRequest();
        }
    }
}