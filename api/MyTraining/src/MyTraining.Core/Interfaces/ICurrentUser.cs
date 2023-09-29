namespace MyTraining.Core.Interfaces;

public interface ICurrentUser
{
    string? UserName { get; }
    string? UserId { get; }
    bool IsAuthenticated();
    bool IsInRole(string role);
}