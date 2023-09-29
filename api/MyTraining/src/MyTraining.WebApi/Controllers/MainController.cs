using Microsoft.AspNetCore.Mvc;
using MyTraining.Application.Shared.Models;
using MyTraining.Core.Interfaces;

namespace MyTraining.API.Controllers;

[ApiController]
public abstract class MainController : ControllerBase
{
    protected readonly ICurrentUser CurrentUser;

    protected MainController(ICurrentUser currentUser)
    {
        CurrentUser = currentUser;
    }
    
    protected ActionResult CustomResponse(Output output)
    {
        if (output.IsValid)
        {
            return Ok(output.Result);
        }

        return BadRequest(output.ErrorMessages);
    }
}