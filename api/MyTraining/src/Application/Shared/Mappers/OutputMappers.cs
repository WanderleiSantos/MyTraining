using Application.UseCases.Exercises.SearchAllExercises.Responses;
using Application.UseCases.Exercises.SearchExerciseById.Responses;
using Application.UseCases.TrainingSheets.InsertTrainingSheet.Responses;
using Application.UseCases.Users.InsertUser.Responses;
using Application.UseCases.Users.SearchUserById.Responses;
using Core.Entities;

namespace Application.Shared.Mappers;

public static class OutputMappers
{
    public static SearchExerciseByIdResponse MapExerciseToSearchExerciseByIdResponse(this Exercise input) => new()
    {
        Id = input.Id,
        Name = input.Name,
        Link = input.Link,
        Active = input.Active
    };
    
    public static InsertUserResponse MapUserToInsertUserResponse(this User input) => new()
    {
        Id = input.Id,
        FirstName = input.FirstName,
        LastName = input.LastName,
        Active = input.Active,
        Email = input.Email
    };

    public static SearchUserByIdResponse MapUserToSearchUserByIdResponse(this User input) => new()
    {
        Id = input.Id,
        FirstName = input.FirstName,
        LastName = input.LastName,
        Active = input.Active,
        Email = input.Email
    };
    
    public static IEnumerable<SearchAllExercisesResponse> MapExercisesToSearchAllExercisesResponse(this IEnumerable<Exercise> inputList)
    {
        return inputList.Select(input => new SearchAllExercisesResponse
        {
            Id = input.Id, 
            Name = input.Name, 
            Link = input.Link, 
            Active = input.Active
        }).ToList();
    }
    
    public static InsertTrainingSheetResponse MapTrainingSheetToInsertTrainingSheetResponse(this TrainingSheet input) => new()
    {
        Id = input.Id,
        UserId = input.UserId,
        Name = input.Name,
        TimeExchange = input.TimeExchange,
        Active = input.Active
    };

}