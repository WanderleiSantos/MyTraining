using Application.UseCases.Exercises.SearchAllExercises.Commands;
using WebApi.V1.Queries;

namespace WebApi.V1.Mappers;

public static class QueryMappers
{
    public static SearchAllExercisesCommand MapToApplication(this SearchAllExercises input, Guid userId) => new SearchAllExercisesCommand
    {
        UserId = userId,
        Name = input.Name,
        Sort = input.Sort,
        PageNumber = input.PageNumber,
        PageSize = input.PageSize
    };
}