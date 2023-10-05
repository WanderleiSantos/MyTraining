using Application.UseCases.Users.InsertUser;
using Application.UseCases.Users.InsertUser.Responses;
using Application.UseCases.Users.SearchUserById;
using Application.UseCases.Users.SearchUserById.Commands;
using Application.UseCases.Users.UpdateUser;
using Core.Interfaces.Extensions;
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
public class UserController : MainController
{
    private readonly ILogger<UserController> _logger;
    private readonly IInsertUserUseCase _insertUserUseCase;
    private readonly ISearchUserByIdUseCase _searchUserByIdUseCase;
    private readonly IUpdateUserUseCase _updateUserUseCase;

    public UserController(ICurrentUser currentUser, ILogger<UserController> logger, IInsertUserUseCase insertUserUseCase, ISearchUserByIdUseCase searchUserByIdUseCase, IUpdateUserUseCase updateUserUseCase) : base(currentUser)
    {
        _logger = logger;
        _insertUserUseCase = insertUserUseCase;
        _searchUserByIdUseCase = searchUserByIdUseCase;
        _updateUserUseCase = updateUserUseCase;
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
            
            return output.IsValid ? 
                CreatedAtAction(nameof(GetById), new {((InsertUserResponse)output.Result!).Id}, output.Result) : 
                CustomResponse(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            return BadRequest();
        }
    }
    
    [HttpGet()]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetById(CancellationToken cancellationToken)
    {
        try
        {
            var command = new SearchUserByIdCommand() { Id = CurrentUser.UserId };
            var output = await _searchUserByIdUseCase.ExecuteAsync(command, cancellationToken);

            return output is { IsValid: true, Result: null } ? NotFound(output.Messages) : CustomResponse(output);
        }
        catch (Exception e)
        { 
            _logger.LogError(e, "An unexpected error occurred");
            return BadRequest();
        }
    }
    
    [HttpPut()]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Update(
        [FromBody] UpdateUserInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await _updateUserUseCase.ExecuteAsync(input.MapToApplication(CurrentUser.UserId), cancellationToken);

            return output.IsValid ? NoContent() : CustomResponse(output);
        }
        catch (Exception e)
        { 
            _logger.LogError(e, "An unexpected error occurred");
            return BadRequest();
        }
    }
}