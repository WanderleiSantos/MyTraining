namespace MyTraining.Application.UseCases.Exercises.SearchAllExercises.Responses;

public class SearchAllExercisesResponse
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Link { get; set; }
    public bool? Active { get; set; }
}