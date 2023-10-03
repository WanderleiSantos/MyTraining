namespace Application.UseCases.Users.InsertUser.Responses;

public class InsertUserResponse
{
    public Guid? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public bool? Active { get; set; }
}