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
    private readonly IInsertUserUseCase _insertUserUseCase;
    private readonly ISearchUserByIdUseCase _searchUserByIdUseCase;
    private readonly IUpdateUserUseCase _updateUserUseCase;
    private readonly IChangeUserPasswordUseCase _changeUserPasswordUseCase;

    public UserController(ICurrentUserService currentUserService, ILogger<UserController> logger, IInsertUserUseCase insertUserUseCase, ISearchUserByIdUseCase searchUserByIdUseCase, IUpdateUserUseCase updateUserUseCase, IChangeUserPasswordUseCase changeUserPasswordUseCase) : base(currentUserService)
    {
        _logger = logger;
        _insertUserUseCase = insertUserUseCase;
        _searchUserByIdUseCase = searchUserByIdUseCase;
        _updateUserUseCase = updateUserUseCase;
        _changeUserPasswordUseCase = changeUserPasswordUseCase;
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
                CreatedAtAction(nameof(GetById), null) : 
                CustomResponse(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, Constants.UnexpectedErrorDescription);
            return InternalServerError(Constants.UnexpectedErrorDescription);
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
        [FromBody] UpdateUserInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await _updateUserUseCase.ExecuteAsync(input.MapToApplication(CurrentUser.UserId), cancellationToken);

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
        [FromBody] ChangeUserPasswordInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await _changeUserPasswordUseCase.ExecuteAsync(input.MapToApplication(CurrentUser.UserId), cancellationToken);

            return CustomResponse(output);
        }
        catch (Exception e)
        { 
            _logger.LogError(e, Constants.UnexpectedErrorDescription);
            return InternalServerError(Constants.UnexpectedErrorDescription);
        }
    }
}