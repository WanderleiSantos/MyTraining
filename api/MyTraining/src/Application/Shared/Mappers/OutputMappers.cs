using Application.UseCases.Exercises.InsertExercise.Responses;
using Application.UseCases.Exercises.SearchAllExercises.Responses;
using Application.UseCases.Exercises.SearchExerciseById.Responses;
using Application.UseCases.TrainingSheets.InsertTrainingSheet.Responses;
using Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries.Responses;
using Application.UseCases.Users.SearchUserById.Responses;
using Core.Entities;

namespace Application.Shared.Mappers;

public static class OutputMappers
{
    public static SearchExerciseByIdResponse MapToResponse(this Exercise input) => new()
    {
        Id = input.Id,
        Name = input.Name,
        Link = input.Link,
        Active = input.Active
    };

    public static SearchUserByIdResponse MapToResponse(this User input) => new()
    {
        Id = input.Id,
        FirstName = input.FirstName,
        LastName = input.LastName,
        Active = input.Active,
        Email = input.Email
    };

    public static IEnumerable<SearchAllExercisesResponse> MapToResponse(this IEnumerable<Exercise> inputList)
    {
        return inputList.Select(input => new SearchAllExercisesResponse
        {
            Id = input.Id,
            Name = input.Name,
            Link = input.Link,
            Active = input.Active
        }).ToList();
    }

    public static InsertTrainingSheetResponse MapToResponse(this TrainingSheet input) => new()
    {
        Id = input.Id,
        UserId = input.UserId,
        Name = input.Name,
        TimeExchange = input.TimeExchange,
        Active = input.Active
    };


    public static InsertExerciseResponse MapToResponseInsertExercise(this Exercise input) => new()
    {
        Id = input.Id,
        Name = input.Name,
        Link = input.Link
    };

    public static InsertTrainingSheetSeriesResponse MapToResponse(this TrainingSheetSeries input) => new()
    {
        Id = input.Id,
        TrainingSheetId = input.TrainingSheetId,
        Name = input.Name,
    };
}