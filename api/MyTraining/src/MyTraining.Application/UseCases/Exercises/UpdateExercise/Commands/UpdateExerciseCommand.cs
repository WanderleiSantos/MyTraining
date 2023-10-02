namespace MyTraining.Application.UseCases.Exercises.UpdateExercise.Commands;

public class UpdateExerciseCommand
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Link { get; set; }
}