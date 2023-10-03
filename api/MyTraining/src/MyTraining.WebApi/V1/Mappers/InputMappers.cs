using MyTraining.API.V1.Models;
using MyTraining.Application.UseCases.Auth.SignIn.Commands;
using MyTraining.Application.UseCases.Exercises.InsertExercise.Commands;
using MyTraining.Application.UseCases.Exercises.UpdateExercise.Commands;
using MyTraining.Application.UseCases.Users.InsertUser.Commands;

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

    public static SignInCommand MapToApplication(this SignInInput input) => new SignInCommand
    {
        Username = input.Username,
        Password = input.Password
    };

    public static InsertExerciseCommand MapToApplication(this InsertExerciseInput input, Guid userId) =>
        new InsertExerciseCommand
        {
            Name = input.Name,
            Link = input.Link,
            UserId = userId
        };

    public static UpdateExerciseCommand MapToApplication(this UpdateExerciseInput input, Guid id) =>
        new UpdateExerciseCommand()
        {
            Id = id,
            Link = input.Link,
            Name = input.Name
        };
}