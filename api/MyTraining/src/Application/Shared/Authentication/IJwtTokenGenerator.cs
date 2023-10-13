namespace Application.Shared.Authentication;

public interface IJwtTokenGenerator
{
    string CreateAccessToken(Guid id, string email);
    string CreateRefreshToken(Guid id, string email);
    (bool, string?) ValidateRefreshToken(string token);
}