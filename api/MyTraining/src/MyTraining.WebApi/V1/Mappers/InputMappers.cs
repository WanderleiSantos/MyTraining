using MyTraining.API.V1.Models;
using MyTraining.Application.UseCases.InsertUser.Commands;
using MyTraining.Application.UseCases.SignIn.Commands;

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
}