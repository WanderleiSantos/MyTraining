using System.Text.Json;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;

namespace Application.UseCases.Users.InsertUser.Services;

public class InitialLoadService : IInitialLoadService
{
    private const string InitialExercisesFileName = "Data/initial_exercises.json";
    private sealed record InitialExercise(string Name, string? Link);

    private readonly IExerciseRepository _repository;
    
    public InitialLoadService(IExerciseRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask InsertExercises(Guid userId, CancellationToken cancellationToken)
    {
        if (!File.Exists(InitialExercisesFileName)) return;
        
        await using var openStream = File.OpenRead(InitialExercisesFileName);
        
        var initialExercises = await JsonSerializer.DeserializeAsync<List<InitialExercise>>(
            openStream, 
            new JsonSerializerOptions{ PropertyNameCaseInsensitive = true }, 
            cancellationToken);

        if (initialExercises != null)
        {
            var exercises = initialExercises.Select(exercise => new Exercise(exercise.Name, exercise.Link, userId)).ToList();

            await _repository.AddRangeAsync(exercises, cancellationToken);
        }
    }
}