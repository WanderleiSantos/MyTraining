using Application.Shared.Models;
using Core.Interfaces.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

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
            return Ok(output.Result);

        return BadRequest(output.ErrorMessages);
    }
}