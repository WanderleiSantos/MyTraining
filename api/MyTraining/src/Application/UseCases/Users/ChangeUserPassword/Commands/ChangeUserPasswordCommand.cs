namespace Application.UseCases.Users.ChangeUserPassword.Commands;

public class ChangeUserPasswordCommand
{
    public Guid Id { get; set; } = Guid.Empty;
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}