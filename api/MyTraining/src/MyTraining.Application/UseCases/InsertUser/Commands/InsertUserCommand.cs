namespace MyTraining.Application.UseCases.InsertUser.Commands;

public class InsertUserCommand
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}