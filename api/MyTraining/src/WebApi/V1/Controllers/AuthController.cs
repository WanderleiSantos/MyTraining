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
    private readonly ISignInUseCase _signInUseCase;
    private readonly IRefreshTokenUseCase _refreshTokenUseCase;

    public AuthController(ICurrentUserService currentUser, ILogger<AuthController> logger, ISignInUseCase signInUseCase, IRefreshTokenUseCase refreshTokenUseCase) : base(currentUser)
    {
        _logger = logger;
        _signInUseCase = signInUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
    }

    [HttpPost("sign-in")]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn(
        [FromBody] SignInInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await _signInUseCase.ExecuteAsync(input.MapToApplication(), cancellationToken);

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
        [FromBody] RefreshTokenInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await _refreshTokenUseCase.ExecuteAsync(input.MapToApplication(), cancellationToken);

            return CustomResponse(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, Constants.UnexpectedErrorDescription);
            return InternalServerError(Constants.UnexpectedErrorDescription);
        }
    }
}