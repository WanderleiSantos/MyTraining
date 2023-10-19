using Application.Shared.Authentication;
using Application.Shared.Models;
using Core.Shared.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApi.Controllers;

[ApiController]
public abstract class MainController : ControllerBase
{
    protected readonly ICurrentUserService CurrentUser;

    protected MainController(ICurrentUserService currentUser)
    {
        CurrentUser = currentUser;
    }
    
    protected IActionResult CustomResponse(Output output)
    {
        return output.IsValid ? Ok(output.Result) : HandleProblem(output);
    }

    protected IActionResult CustomResponseCreatedAtAction(Output output, string actionName, object? routeValues)
    {
        return output.IsValid ? CreatedAtAction(actionName, routeValues, output.Result) : HandleProblem(output);
    }
    
    protected IActionResult InternalServerError(string description)
    {
        return Problem(statusCode: StatusCodes.Status500InternalServerError, title: description);
    }
    
    private IActionResult HandleProblem(Output output)
    {
        return output.Errors.First().Type == ErrorType.Validation ? ValidationProblems(output.Errors) : Problems(output.Errors.First());
    }
    
    private IActionResult Problems(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(statusCode: statusCode, title: error.Description);
    }
    
    private IActionResult ValidationProblems(IEnumerable<Error> errors)
    {
        var modelStateDictionary = new ModelStateDictionary();
        foreach (var error in errors)
        {
            modelStateDictionary.AddModelError(error.Code, error.Description);
        }
        return ValidationProblem(modelStateDictionary);
    }
}