namespace Application.Shared.Authentication;

public interface ICurrentUserService
{
    string? UserEmail { get; }
    Guid UserId { get; }
    bool IsAuthenticated();
    bool IsInRole(string role);
}