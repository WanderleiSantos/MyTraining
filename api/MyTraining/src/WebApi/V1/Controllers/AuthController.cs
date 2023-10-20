using Application.Shared.Authentication;
using Application.UseCases.Auth.RefreshToken;
using Application.UseCases.Auth.SignIn;
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
public class AuthController : MainController
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ICurrentUserService currentUser, ILogger<AuthController> logger) : base(currentUser)
    {
        _logger = logger;
    }

    [HttpPost("sign-in")]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn(
        [FromServices] ISignInUseCase signInUseCase,
        [FromBody] SignInInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await signInUseCase.ExecuteAsync(input.MapToApplication(), cancellationToken);

            return CustomResponse(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, Constants.UnexpectedErrorDescription);
            return InternalServerError(Constants.UnexpectedErrorDescription);
        }
    }
    
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(
        [FromServices] IRefreshTokenUseCase refreshTokenUseCase,
        [FromBody] RefreshTokenInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await refreshTokenUseCase.ExecuteAsync(input.MapToApplication(), cancellationToken);

            return CustomResponse(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, Constants.UnexpectedErrorDescription);
            return InternalServerError(Constants.UnexpectedErrorDescription);
        }
    }
}