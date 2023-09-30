namespace MyTraining.Application.UseCases.SignIn.Responses;

public class SignInResponse
{
    public string? UserName { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
}