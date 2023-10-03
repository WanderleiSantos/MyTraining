using Application.UseCases.Auth.SignIn;
using Core.Interfaces.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;
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

    public AuthController(ICurrentUser currentUser, ILogger<AuthController> logger, ISignInUseCase signInUseCase) : base(currentUser)
    {
        _logger = logger;
        _signInUseCase = signInUseCase;
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
            _logger.LogError(ex, "An unexpected error occurred.");
            return BadRequest();
        }
    }
}