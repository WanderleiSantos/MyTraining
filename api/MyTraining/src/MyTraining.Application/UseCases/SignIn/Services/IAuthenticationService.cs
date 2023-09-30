namespace MyTraining.Application.UseCases.SignIn.Services;

public interface IAuthenticationService
{
    string CreateToken(Guid id, string username);
}