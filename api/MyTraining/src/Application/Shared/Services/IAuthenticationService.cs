namespace Application.Shared.Services;

public interface IAuthenticationService
{
    string CreateAccessToken(Guid id, string email);
    string CreateRefreshToken(Guid id, string email);
    (bool, string?) ValidateRefreshToken(string token);
}