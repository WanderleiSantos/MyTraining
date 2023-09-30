using MyTraining.API.V1.Models;
using MyTraining.Application.UseCases.InsertExercise.Commands;
using MyTraining.Application.UseCases.InsertUser.Commands;

namespace MyTraining.API.V1.Mappers;

public static class InputMappers
{
    public static InsertUserCommand MapToApplication(this InsertUserInput input) => new InsertUserCommand
    {
        FirstName = input.FirstName,
        LastName = input.LastName,
        Email = input.Email,
        Password = input.Password
    };

    public static InsertExerciseCommand MapToApplication(this InsertExerciseInput input, Guid userId) => new InsertExerciseCommand
    {
        Name = input.Name,
        Link = input.Link,
        UserId = userId
    };
}