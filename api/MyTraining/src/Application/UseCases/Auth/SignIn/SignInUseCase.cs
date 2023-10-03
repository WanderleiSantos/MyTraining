using Application.Shared.Models;
using Application.Shared.Services;
using Application.UseCases.Auth.SignIn.Commands;
using Application.UseCases.Auth.SignIn.Responses;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Application.Shared.Extensions;
using Core.Interfaces.Persistence.Repositories;

namespace Application.UseCases.Auth.SignIn;

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

            _logger.LogInformation("{UseCase} - Getting user; Username: {Username}",
                nameof(SignInUseCase), command.Username);

            var user = await _repository.GetByEmailAsync(command.Username, cancellationToken);

            if (user == null || !command.Password.VerifyPassword(user.Password))
            {
                output.AddErrorMessage("User does not exist");
                _logger.LogWarning("{UseCase} - User does not exist", nameof(SignInUseCase));
                return output;
            }
            
            if (!user.Active)
            {
                output.AddErrorMessage("Inactive user");
                _logger.LogWarning("{UseCase} - Inactive user; Username: {Username}", 
                    nameof(SignInUseCase), command.Username);
                return output;
            }

            _logger.LogInformation("{UseCase} - Generating authentication token; Username: {Username}",
                nameof(SignInUseCase), command.Username);

            var token = _authenticationService.CreateAccessToken(user.Id, user.Email);
            var refreshToken = _authenticationService.CreateRefreshToken(user.Id, user.Email);
            var response = new SignInResponse
            {
                UserName = user.Email,
                Token = token,
                RefreshToken = refreshToken
            };
            
            output.AddResult(response);

            _logger.LogInformation("{UseCase} - Token generated successfully; Username: {Username}",
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