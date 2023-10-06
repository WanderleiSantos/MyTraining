namespace Core.Interfaces.Services;

public interface ICurrentUserService
{
    string? UserName { get; }
    Guid UserId { get; }
    bool IsAuthenticated();
    bool IsInRole(string role);
}