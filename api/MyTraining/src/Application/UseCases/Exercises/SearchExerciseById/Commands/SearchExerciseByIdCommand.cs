namespace Application.UseCases.Exercises.SearchExerciseById.Commands;

public class SearchExerciseByIdCommand
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}