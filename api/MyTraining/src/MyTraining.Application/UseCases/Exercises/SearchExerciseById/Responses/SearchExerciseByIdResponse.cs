namespace MyTraining.Application.UseCases.Exercises.SearchExerciseById.Responses;

public class SearchExerciseByIdResponse
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Link { get; set; }
    public bool? Active { get; set; }
}