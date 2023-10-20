using Application.Shared.Authentication;
using Application.UseCases.Users.ChangeUserPassword;
using Application.UseCases.Users.InsertUser;
using Application.UseCases.Users.SearchUserById;
using Application.UseCases.Users.SearchUserById.Commands;
using Application.UseCases.Users.UpdateUser;
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
public class UserController : MainController
{
    private readonly ILogger<UserController> _logger;

    public UserController(ICurrentUserService currentUserService, ILogger<UserController> logger) : base(currentUserService)
    {
        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create(
        [FromServices] IInsertUserUseCase insertUserUseCase,
        [FromBody] InsertUserInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await insertUserUseCase.ExecuteAsync(input.MapToApplication(), cancellationToken);
            
            return CustomResponseCreatedAtAction(output, nameof(GetById), null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, Constants.UnexpectedErrorDescription);
            return InternalServerError(Constants.UnexpectedErrorDescription);
        }
    }
    
    [HttpGet()]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetById(
        [FromServices] ISearchUserByIdUseCase searchUserByIdUseCase,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new SearchUserByIdCommand() { Id = CurrentUser.UserId };
            var output = await searchUserByIdUseCase.ExecuteAsync(command, cancellationToken);

            return CustomResponse(output);
        }
        catch (Exception e)
        { 
            _logger.LogError(e, Constants.UnexpectedErrorDescription);
            return InternalServerError(Constants.UnexpectedErrorDescription);
        }
    }
    
    [HttpPut()]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Update(
        [FromServices] IUpdateUserUseCase updateUserUseCase,
        [FromBody] UpdateUserInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await updateUserUseCase.ExecuteAsync(input.MapToApplication(CurrentUser.UserId), cancellationToken);

            return CustomResponse(output);
        }
        catch (Exception e)
        { 
            _logger.LogError(e, Constants.UnexpectedErrorDescription);
            return InternalServerError(Constants.UnexpectedErrorDescription);
        }
    }
    
    [HttpPut( "password")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangePassword(
        [FromServices] IChangeUserPasswordUseCase changeUserPasswordUseCase,
        [FromBody] ChangeUserPasswordInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await changeUserPasswordUseCase.ExecuteAsync(input.MapToApplication(CurrentUser.UserId), cancellationToken);

            return CustomResponse(output);
        }
        catch (Exception e)
        { 
            _logger.LogError(e, Constants.UnexpectedErrorDescription);
            return InternalServerError(Constants.UnexpectedErrorDescription);
        }
    }
}