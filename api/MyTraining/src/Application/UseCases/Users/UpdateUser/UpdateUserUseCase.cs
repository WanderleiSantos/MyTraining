using Application.Shared.Models;
using Application.UseCases.Users.UpdateUser.Commands;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Core.Interfaces.Persistence.Repositories;
using Core.Shared.Errors;
using Errors = Core.Shared.Errors.Errors;

namespace Application.UseCases.Users.UpdateUser;

public class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly ILogger<UpdateUserUseCase> _logger;
    private readonly IUserRepository _repository;
    private readonly IValidator<UpdateUserCommand> _validator;

    public UpdateUserUseCase(ILogger<UpdateUserUseCase> logger, IUserRepository repository, IValidator<UpdateUserCommand> validator)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Output> ExecuteAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var output = new Output();
        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);

            output.AddValidationResult(validationResult);

            if (!output.IsValid)
                return output;
            
            _logger.LogInformation("{UseCase} - Search user by id: {id};", nameof(UpdateUserUseCase), command.Id);

            var user = await _repository.GetByIdAsync(command.Id, cancellationToken);
            if (user is null)
            {
                _logger.LogWarning("{UseCase} - User does not exist; Id: {id};", nameof(UpdateUserCommand), command.Id);
                
                output.AddError(Errors.User.DoesNotExist);
                return output;
            }
            
            _logger.LogInformation("{UseCase} - Updating User by id: {id};", nameof(UpdateUserCommand), command.Id);

            user.Update(command.FirstName, command.LastName);
            
            await _repository.UnitOfWork.CommitAsync();
            
            _logger.LogInformation("{UseCase} - User updated successfully; Id: {id};", nameof(UpdateUserCommand), command.Id);

            output.AddResult(null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{UseCase} - An unexpected error has occurred;", nameof(UpdateUserCommand));

            output.AddError(Error.Unexpected());
        }

        return output;
    }
}