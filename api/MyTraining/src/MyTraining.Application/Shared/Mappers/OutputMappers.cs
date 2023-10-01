using MyTraining.Application.UseCases.Exercises.SearchAllExercises.Responses;
using MyTraining.Application.UseCases.Exercises.SearchExerciseById.Responses;
using MyTraining.Application.UseCases.Users.SearchUserById.Responses;
using MyTraining.Core.Entities;

namespace MyTraining.Application.Shared.Mappers;

public static class OutputMappers
{
    public static SearchExerciseByIdResponse MapToApplication(this Exercise input) => new()
    {
        Id = input.Id,
        Name = input.Name,
        Link = input.Link,
        Active = input.Active
    };

    public static SearchUserByIdResponse MapToApplication(this User input) => new()
    {
        Id = input.Id,
        FirstName = input.FirstName,
        LastName = input.LastName,
        Active = input.Active,
        Email = input.Email
    };
    
    public static List<SearchAllExercisesResponse> MapToApplication(this IEnumerable<Exercise> inputList)
    {
        return inputList.Select(input => new SearchAllExercisesResponse
            { Id = input.Id, Name = input.Name, Link = input.Link, Active = input.Active }).ToList();
    }
}