using Application.Shared.Models;
using Application.Shared.Services;
using Application.UseCases.Auth.RefreshToken.Commands;
using Application.UseCases.Auth.RefreshToken.Responses;
using Core.Interfaces.Persistence.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Auth.RefreshToken;

public class RefreshTokenUseCase : IRefreshTokenUseCase
{
    private readonly ILogger<RefreshTokenUseCase> _logger;
    private readonly IUserRepository _repository;
    private readonly IValidator<RefreshTokenCommand> _validator;
    private readonly IAuthenticationService _authenticationService;

    public RefreshTokenUseCase(ILogger<RefreshTokenUseCase> logger, IUserRepository repository, IValidator<RefreshTokenCommand> validator, IAuthenticationService authenticationService)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
        _authenticationService = authenticationService;
    }

    public async Task<Output> ExecuteAsync(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var output = new Output();

        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);

            output.AddValidationResult(validationResult);

            if (!output.IsValid)
                return output;

            _logger.LogInformation("{UseCase} - Validating token", nameof(RefreshTokenUseCase));

            var (validToken, username) = _authenticationService.ValidateRefreshToken(command.RefreshToken);

            if (!validToken || username == null)
            {
                output.AddErrorMessage("Token is expired or User is not valid");
                _logger.LogWarning("{UseCase} - Token is expired or User is not valid", nameof(RefreshTokenUseCase));
                return output;
            }
            
            var user = await _repository.GetByEmailAsync(username, cancellationToken);

            if (user is not { Active: true })
            {
                output.AddErrorMessage("User does not exist or inactive");
                _logger.LogWarning("{UseCase} - User does not exist or inactive; Username {username}", 
                    nameof(RefreshTokenUseCase), username);
                return output;
            }

            _logger.LogInformation("{UseCase} - Generating authentication token; Username: {Username}",
                nameof(RefreshTokenUseCase), username);

            var token = _authenticationService.CreateAccessToken(user.Id, user.Email);
            var refreshToken = _authenticationService.CreateRefreshToken(user.Id, user.Email);
            var response = new RefreshTokenResponse()
            {
                Token = token,
                RefreshToken = refreshToken
            };
            
            output.AddResult(response);

            _logger.LogInformation("{UseCase} - Token generated successfully; Username: {Username}",
                nameof(RefreshTokenUseCase), username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{UseCase} - An unexpected error has occurred;",
                nameof(RefreshTokenUseCase));

            output.AddErrorMessage("An unexpected error has occurred");
        }

        return output;
    }
}