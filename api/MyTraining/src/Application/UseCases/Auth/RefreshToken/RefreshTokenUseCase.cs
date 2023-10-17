using Application.Shared.Authentication;
using Application.Shared.Models;
using Application.UseCases.Auth.RefreshToken.Commands;
using Application.UseCases.Auth.RefreshToken.Responses;
using Core.Interfaces.Persistence.Repositories;
using Core.Shared.Errors;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Errors = Core.Shared.Errors.Errors;

namespace Application.UseCases.Auth.RefreshToken;

public class RefreshTokenUseCase : IRefreshTokenUseCase
{
    private readonly ILogger<RefreshTokenUseCase> _logger;
    private readonly IUserRepository _repository;
    private readonly IValidator<RefreshTokenCommand> _validator;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenUseCase(ILogger<RefreshTokenUseCase> logger, IUserRepository repository, IValidator<RefreshTokenCommand> validator, IJwtTokenGenerator jwtTokenGenerator)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
        _jwtTokenGenerator = jwtTokenGenerator;
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

            _logger.LogInformation("{UseCase} - Validating token;", nameof(RefreshTokenUseCase));

            var (validToken, email) = _jwtTokenGenerator.ValidateRefreshToken(command.RefreshToken);

            if (!validToken || email == null)
            {
                _logger.LogWarning("{UseCase} - Token is expired or User is not valid;", nameof(RefreshTokenUseCase));

                output.AddError(Errors.Authentication.InvalidToken);
                return output;
            }
            
            var user = await _repository.GetByEmailAsync(email, cancellationToken);

            if (user is not { Active: true })
            {
                _logger.LogWarning("{UseCase} - User does not exist or inactive; Email {Email};", nameof(RefreshTokenUseCase), email);

                output.AddError(Errors.Authentication.UserInactive);
                return output;
            }

            _logger.LogInformation("{UseCase} - Generating authentication token; Email: {Email};", nameof(RefreshTokenUseCase), email);
            
            output.AddResult(new RefreshTokenResponse()
            {
                Token = _jwtTokenGenerator.CreateAccessToken(user.Id, user.Email),
                RefreshToken = _jwtTokenGenerator.CreateRefreshToken(user.Id, user.Email)
            });

            _logger.LogInformation("{UseCase} - Token generated successfully; Email: {Email};", nameof(RefreshTokenUseCase), email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{UseCase} - An unexpected error has occurred;", nameof(RefreshTokenUseCase));

            output.AddError(Error.Unexpected());
        }

        return output;
    }
}