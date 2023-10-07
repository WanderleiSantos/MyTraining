namespace WebApi.V1.Models;

public class ChangeUserPasswordInput
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}