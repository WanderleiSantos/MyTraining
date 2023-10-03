namespace WebApi.V1.Models;

public class SignInInput
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}