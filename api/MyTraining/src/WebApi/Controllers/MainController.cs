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
            ErrorType.Validation => BadRequest(output.ErrorMessages),
            ErrorType.Conflict => Conflict(output.ErrorMessages),
            ErrorType.NotFound => NotFound(output.ErrorMessages),
            ErrorType.Unauthorized => Unauthorized(output.ErrorMessages),
            _ => InternalServerError(output.ErrorMessages)
        };
    }

    protected ActionResult InternalServerError(object? error)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, error);
    }
}