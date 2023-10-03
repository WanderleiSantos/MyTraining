namespace Application.Shared.Services;

public interface IAuthenticationService
{
    string CreateAccessToken(Guid id, string username);
    string CreateRefreshToken(Guid id, string username);
}