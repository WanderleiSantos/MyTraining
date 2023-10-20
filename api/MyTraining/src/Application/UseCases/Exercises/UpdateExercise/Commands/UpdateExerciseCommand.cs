namespace Application.UseCases.Exercises.UpdateExercise.Commands;

public class UpdateExerciseCommand
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Link { get; set; }
    public Guid UserId { get;  set; }
}