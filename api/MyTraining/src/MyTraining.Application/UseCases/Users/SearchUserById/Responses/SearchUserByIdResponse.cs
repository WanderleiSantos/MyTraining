namespace MyTraining.Application.UseCases.Users.SearchUserById.Responses;

public class SearchUserByIdResponse
{
    public Guid? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public bool? Active { get; set; }
}