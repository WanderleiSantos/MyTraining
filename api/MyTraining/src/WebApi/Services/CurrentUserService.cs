using System.Security.Claims;
using Core.Interfaces.Services;

namespace WebApi.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly ClaimsIdentity? _identity;
    
    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
        _identity = _accessor.HttpContext?.User.Identity as ClaimsIdentity;
    }
    
    public string? UserName => _identity?.FindFirst(ClaimTypes.Name)?.Value;
    public Guid UserId => IsAuthenticated() ? Guid.Parse(_identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty) : Guid.Empty;
    
    public bool IsAuthenticated() => _identity is { IsAuthenticated: true };
    
    public bool IsInRole(string role) => _accessor.HttpContext != null && _accessor.HttpContext.User.IsInRole(role);
}