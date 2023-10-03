namespace Application.UseCases.Auth.SignIn.Commands;

public class SignInCommand
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}