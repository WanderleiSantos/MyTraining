using Application.Shared.Models;
using Core.Common.Errors;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common.Error;

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
            ErrorType.Validation => BadRequest(ErrorMapping(output.Errors)),
            ErrorType.Conflict => Conflict(ErrorMapping(output.Errors)),
            ErrorType.NotFound => NotFound(ErrorMapping(output.Errors)),
            ErrorType.Unauthorized => Unauthorized(ErrorMapping(output.Errors)),
            _ => InternalServerError(ErrorMapping(output.Errors))
        };
    }

    protected ActionResult InternalServerError(object? error)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, error);
    }

    private static List<ErrorOutput> ErrorMapping(IEnumerable<Error> errors)
    {
        return errors.Select(e => new ErrorOutput(e.Code, e.Description)).ToList();
    }
}