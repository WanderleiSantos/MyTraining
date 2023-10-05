namespace WebApi.V1.Models;

public class SignInInput
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}