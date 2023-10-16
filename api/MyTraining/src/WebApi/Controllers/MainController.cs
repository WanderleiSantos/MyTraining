using Application.Shared.Models;
using Core.Common.Errors;
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

        return output.FirstError switch
        {
            null => BadRequest(output.Errors),
            ErrorType.Validation => BadRequest(output.Errors),
            ErrorType.Conflict => Conflict(output.Errors),
            ErrorType.NotFound => NotFound(output.Errors),
            ErrorType.Unauthorized => Unauthorized(output.Errors),
            _ => InternalServerError(output.Errors)
        };
    }

    protected ActionResult InternalServerError(object? error)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, error);
    }
}