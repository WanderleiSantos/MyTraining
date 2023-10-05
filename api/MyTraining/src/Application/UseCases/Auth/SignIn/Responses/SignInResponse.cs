namespace Application.UseCases.Auth.SignIn.Responses;

public class SignInResponse
{
    public string? Email { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
}