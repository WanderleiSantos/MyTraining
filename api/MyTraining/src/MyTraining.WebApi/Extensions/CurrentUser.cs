using System.Security.Claims;
using System.Security.Principal;
using MyTraining.Core.Interfaces;
using MyTraining.Core.Interfaces.Extensions;

namespace MyTraining.API.Extensions;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;
    private readonly ClaimsIdentity? _identity;
    
    public CurrentUser(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
        _identity = _accessor.HttpContext?.User.Identity as ClaimsIdentity;
    }
    
    public string? UserName => _identity?.FindFirst(ClaimTypes.Name)?.Value;
    public string? UserId => IsAuthenticated() ? _identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value : string.Empty;
    
    public bool IsAuthenticated() => _identity is { IsAuthenticated: true };
    
    public bool IsInRole(string role) => _accessor.HttpContext != null && _accessor.HttpContext.User.IsInRole(role);
}