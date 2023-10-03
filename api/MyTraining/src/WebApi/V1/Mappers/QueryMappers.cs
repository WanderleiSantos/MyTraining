using Application.UseCases.Exercises.SearchAllExercises.Commands;
using WebApi.V1.Queries;

namespace WebApi.V1.Mappers;

public static class QueryMappers
{
    public static SearchAllExercisesCommand MapToApplication(this SearchAllExercisesQuery input, Guid userId) => new SearchAllExercisesCommand
    {
        UserId = userId,
        PageNumber = input.PageNumber,
        PageSize = input.PageSize
    };
}