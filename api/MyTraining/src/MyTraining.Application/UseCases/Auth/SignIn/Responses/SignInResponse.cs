namespace MyTraining.Application.UseCases.Auth.SignIn.Responses;

public class SignInResponse
{
    public string? UserName { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
}