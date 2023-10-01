namespace MyTraining.Application.UseCases.SearchUserById.Commands;

public class SearchUserByIdCommand
{
    public Guid Id { get; set; } = Guid.Empty;
}