using Application.Shared.Authentication;
using Application.Shared.Models;
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
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public SignInUseCase(ILogger<SignInUseCase> logger, IUserRepository repository, IValidator<SignInCommand> validator, IJwtTokenGenerator jwtTokenGenerator)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
        _jwtTokenGenerator = jwtTokenGenerator;
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

            _logger.LogInformation("{UseCase} - Getting user; Email: {Email}", nameof(SignInUseCase), command.Email);

            var user = await _repository.GetByEmailAsync(command.Email, cancellationToken);

            if (user == null || !command.Password.VerifyPassword(user.Password))
            {
                _logger.LogWarning("{UseCase} - User does not exist", nameof(SignInUseCase));
                
                output
                    .AddError("User does not exist")
                    .SetErrorType(ErrorType.Unauthorized);
                return output;
            }
            
            if (!user.Active)
            {
                _logger.LogWarning("{UseCase} - Inactive user; Email: {Email}", nameof(SignInUseCase), command.Email);
                
                output
                    .AddError("Inactive user")
                    .SetErrorType(ErrorType.Unauthorized);
                return output;
            }

            _logger.LogInformation("{UseCase} - Generating authentication token; Email: {Email}", nameof(SignInUseCase), command.Email);

            var token = _jwtTokenGenerator.CreateAccessToken(user.Id, user.Email);
            var refreshToken = _jwtTokenGenerator.CreateRefreshToken(user.Id, user.Email);
            var response = new SignInResponse
            {
                Email = user.Email,
                Token = token,
                RefreshToken = refreshToken
            };
            
            output.AddResult(response);

            _logger.LogInformation("{UseCase} - Token generated successfully; Email: {Email}", nameof(SignInUseCase), command.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{UseCase} - An unexpected error has occurred;", nameof(SignInUseCase));

            output
                .AddError("An unexpected error has occurred")
                .SetErrorType(ErrorType.Unexpected);
        }

        return output;
    }
}