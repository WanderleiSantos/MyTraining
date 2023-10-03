namespace Core.Interfaces.Extensions;

public interface ICurrentUser
{
    string? UserName { get; }
    Guid UserId { get; }
    bool IsAuthenticated();
    bool IsInRole(string role);
}