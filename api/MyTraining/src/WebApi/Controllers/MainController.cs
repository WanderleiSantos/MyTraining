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

        return BadRequest(output.ErrorMessages);
    }
}