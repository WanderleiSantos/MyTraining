using FluentValidation;
using Microsoft.Extensions.Logging;
using MyTraining.Application.Shared.Extensions;
using MyTraining.Application.Shared.Models;
using MyTraining.Application.UseCases.SignIn.Commands;
using MyTraining.Application.UseCases.SignIn.Services;
using MyTraining.Core.Interfaces.Persistence.Repositories;

namespace MyTraining.Application.UseCases.SignIn;

public class SignInUseCase : ISignInUseCase
{
    private readonly ILogger<SignInUseCase> _logger;
    private readonly IUserRepository _repository;
    private readonly IValidator<SignInCommand> _validator;
    private readonly IAuthenticationService _authenticationService;

    public SignInUseCase(ILogger<SignInUseCase> logger, IUserRepository repository, IValidator<SignInCommand> validator, IAuthenticationService authenticationService)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
        _authenticationService = authenticationService;
    }

    public async Task<Output> ExecuteAsync(SignInCommand command, CancellationToken cancellationToken)
    {
        var output = new Output();

        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);

            output.AddValidationResult(validationResult);

            if (!output.IsValid)
                return output;

            _logger.LogInformation("{UseCase} - Getting user; Name: {Username}",
                nameof(SignInUseCase), command.Username);

            var user = await _repository.GetByEmailAsync(command.Username, cancellationToken);

            if (user == null || user.Password != command.Password.CreateSHA256Hash())
            {
                output.AddErrorMessage("User does not exist");
                _logger.LogWarning("User does not exist");
                return output;
            }

            _logger.LogInformation("{UseCase} - Generating authentication token; Name: {Username}",
                nameof(SignInUseCase), command.Username);

            var token = _authenticationService.CreateToken(user.Id, user.Email);
            output.AddResult(token);

            _logger.LogInformation("{UseCase} - Token generated successfully; Name: {Username}",
                nameof(SignInUseCase), command.Username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{UseCase} - An unexpected error has occurred;",
                nameof(SignInUseCase));

            output.AddErrorMessage("An unexpected error has occurred");
        }

        return output;
    }
}