using Application.UseCases.Auth.RefreshToken.Commands;
using Application.UseCases.Auth.SignIn.Commands;
using Application.UseCases.Exercises.InsertExercise.Commands;
using Application.UseCases.Exercises.UpdateExercise.Commands;
using Application.UseCases.TrainingSheets.InsertTrainingSheet.Commands;
using Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries.Commands;
using Application.UseCases.Users.ChangeUserPassword.Commands;
using Application.UseCases.Users.InsertUser.Commands;
using Application.UseCases.Users.UpdateUser.Commands;
using WebApi.V1.Models;

namespace WebApi.V1.Mappers;

public static class InputMappers
{
    public static InsertUserCommand MapToApplication(this InsertUserInput input) => new InsertUserCommand
    {
        FirstName = input.FirstName,
        LastName = input.LastName,
        Email = input.Email,
        Password = input.Password
    };

    public static UpdateUserCommand MapToApplication(this UpdateUserInput input, Guid id) => new UpdateUserCommand
    {
        Id = id,
        FirstName = input.FirstName,
        LastName = input.LastName,
    };

    public static ChangeUserPasswordCommand MapToApplication(this ChangeUserPasswordInput input, Guid id) =>
        new ChangeUserPasswordCommand
        {
            Id = id,
            OldPassword = input.OldPassword,
            NewPassword = input.NewPassword
        };

    public static SignInCommand MapToApplication(this SignInInput input) => new SignInCommand
    {
        Email = input.Email,
        Password = input.Password
    };

    public static RefreshTokenCommand MapToApplication(this RefreshTokenInput input) => new RefreshTokenCommand
    {
        RefreshToken = input.RefreshToken
    };

    public static InsertExerciseCommand MapToApplication(this InsertExerciseInput input, Guid userId) =>
        new InsertExerciseCommand
        {
            Name = input.Name,
            Link = input.Link,
            UserId = userId
        };

    public static UpdateExerciseCommand MapToApplication(this UpdateExerciseInput input, Guid id, Guid userId) =>
        new UpdateExerciseCommand()
        {
            Id = id,
            Link = input.Link,
            Name = input.Name,
            UserId = userId
        };

    public static InsertTrainingSheetCommand MapToApplication(this InsertTrainingSheetInput input, Guid userId) =>
        new InsertTrainingSheetCommand()
        {
            Name = input.Name,
            TimeExchange = input.TimeExchange,
            UserId = userId
        };

    public static InsertTrainingSheetSeriesCommand MapToApplication(this InsertTrainingSheetSeriesInput input,
        Guid userId) =>
        new InsertTrainingSheetSeriesCommand()
        {
            Name = input.Name,
            TrainingSheetId = input.TrainingSheetId
        };
}