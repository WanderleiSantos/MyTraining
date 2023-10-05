namespace Application.UseCases.Auth.RefreshToken.Responses;

public class RefreshTokenResponse
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
}