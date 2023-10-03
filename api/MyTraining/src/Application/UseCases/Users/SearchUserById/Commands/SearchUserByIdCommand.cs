namespace Application.UseCases.Users.SearchUserById.Commands;

public class SearchUserByIdCommand
{
    public Guid Id { get; set; } = Guid.Empty;
}