using MyTraining.API.V1.Queries;
using MyTraining.Application.UseCases.Exercises.SearchAllExercises.Commands;

namespace MyTraining.API.V1.Mappers;

public static class QueryMappers
{
    public static SearchAllExercisesCommand MapToApplication(this SearchAllExercisesQuery input, Guid userId) => new SearchAllExercisesCommand
    {
        UserId = userId,
        PageNumber = input.PageNumber,
        PageSize = input.PageSize
    };
}