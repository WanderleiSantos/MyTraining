namespace Application.UseCases.Users.InsertUser.Services;

public interface IInitialLoadService
{
    ValueTask InsertExercises(Guid userId, CancellationToken cancellationToken);
}