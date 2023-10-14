namespace Core.Entities;

public class PlanningExercises : BaseEntity
{
    public PlanningExercises()
    {
    }

    public PlanningExercises(Guid exerciseId, Guid seriesPlaningId)
    {
        ExerciseId = exerciseId;
        SeriesPlaningId = seriesPlaningId;
    }

    public Guid ExerciseId { get; private set; }
    public Guid SeriesPlaningId { get; private set; }
    public Exercise Exercise { get; private set; }
    
}