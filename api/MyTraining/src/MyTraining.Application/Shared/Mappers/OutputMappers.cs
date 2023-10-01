using MyTraining.Application.UseCases.SearchExerciseById.Responses;
using MyTraining.Application.UseCases.SearchUserById.Responses;
using MyTraining.Core.Entities;

namespace MyTraining.Application.Shared.Mappers;

public static class OutputMappers
{
    public static SearchExerciseByIdResponse MapToApplication(this Exercise input) => new()
    {
        Id = input.Id,
        Name = input.Name,
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
}