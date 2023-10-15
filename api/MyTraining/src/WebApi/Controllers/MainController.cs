using Application.Shared.Models;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
public abstract class MainController : ControllerBase
{
    protected readonly ICurrentUserService CurrentUserService;

    protected MainController(ICurrentUserService currentUserService)
    {
        CurrentUserService = currentUserService;
    }
    
    protected ActionResult CustomResponse(Output output)
    {
        if (output.IsValid)
            return Ok(output.Result);

        return output.ErrorType switch
        {
            null => BadRequest(output.ErrorMessages),
            EErrorType.Validation => BadRequest(output.ErrorMessages),
            EErrorType.Conflict => Conflict(output.ErrorMessages),
            EErrorType.NotFound => NotFound(output.ErrorMessages),
            EErrorType.Unauthorized => Unauthorized(output.ErrorMessages),
            _ => InternalServerError(output.ErrorMessages)
        };
    }

    protected ActionResult InternalServerError(object? error)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, error);
    }
}