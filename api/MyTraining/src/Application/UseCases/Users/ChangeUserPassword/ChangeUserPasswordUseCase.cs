using Application.Shared.Extensions;
using Application.Shared.Models;
using Application.UseCases.Users.ChangeUserPassword.Commands;
using Core.Common.Errors;
using Core.Interfaces.Persistence.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Users.ChangeUserPassword;

public class ChangeUserPasswordUseCase : IChangeUserPasswordUseCase
{
    private readonly ILogger<ChangeUserPasswordUseCase> _logger;
    private readonly IUserRepository _repository;
    private readonly IValidator<ChangeUserPasswordCommand> _validator;

    public ChangeUserPasswordUseCase(ILogger<ChangeUserPasswordUseCase> logger, IUserRepository repository, IValidator<ChangeUserPasswordCommand> validator)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Output> ExecuteAsync(ChangeUserPasswordCommand command, CancellationToken cancellationToken)
    {
        var output = new Output();
        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);

            output.AddValidationResult(validationResult);

            if (!output.IsValid)
                return output;
            
            _logger.LogInformation("{UseCase} - Search user by id: {id}", nameof(ChangeUserPasswordUseCase), command.Id);

            var user = await _repository.GetByIdAsync(command.Id, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User does not exist");

                output.AddError(Errors.User.DoesNotExist);
                return output;
            }
            
            if (!command.OldPassword.VerifyPassword(user.Password))
            {
                _logger.LogWarning("{UseCase} - Old password does not match", nameof(ChangeUserPasswordUseCase));
                
                output.AddError(Errors.Authentication.InvalidPassword);
                return output;
            }
            
            _logger.LogInformation("{UseCase} - Updating User password by id: {id}", nameof(ChangeUserPasswordUseCase), command.Id);

            user.UpdatePassword(command.NewPassword.HashPassword());
            
            await _repository.UnitOfWork.CommitAsync();
            
            _logger.LogInformation("{UseCase} - User password updated successfully; Id: {id}", nameof(ChangeUserPasswordUseCase), command.Id);

            output.AddResult(null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{UseCase} - An unexpected error has occurred;", nameof(ChangeUserPasswordUseCase));

            output.AddError(Error.Unexpected());
        }

        return output;
    }
}