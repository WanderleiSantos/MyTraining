using Application.Shared.Models;
using Application.UseCases.Users.InsertUser.Commands;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Application.Shared.Extensions;
using Application.Shared.Mappers;
using Core.Entities;
using Core.Interfaces.Persistence.Repositories;
using Infrastructure.Persistence;

namespace Application.UseCases.Users.InsertUser;

public class InsertUserUseCase : IInsertUserUseCase
{
    private readonly ILogger<InsertUserUseCase> _logger;
    private readonly IUserRepository _repository;
    private readonly IValidator<InsertUserCommand> _validator;

    public InsertUserUseCase(ILogger<InsertUserUseCase> logger, IUserRepository repository, IValidator<InsertUserCommand> validator)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
    }
    
    public async Task<Output> ExecuteAsync(InsertUserCommand command, CancellationToken cancellationToken)
    {
        var output = new Output();
        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);

            output.AddValidationResult(validationResult);

            if (!output.IsValid)
                return output;

            if (await _repository.ExistsEmailRegisteredAsync(command.Email, cancellationToken))
            {
                output.AddErrorMessage(new Notification("Email","E-mail already registered"));
                _logger.LogWarning("{UseCase} - E-mail already registered; Email {email}", 
                    nameof(InsertUserUseCase), command.Email);
                return output;
            }

            _logger.LogInformation("{UseCase} - Inserting user; Email: {Email}",
                nameof(InsertUserUseCase), command.Email);

            var result = new User(command.FirstName, command.LastName, command.Email, command.Password.HashPassword());

            await _repository.AddAsync(result, cancellationToken);

            await _repository.UnitOfWork.CommitAsync();

            _logger.LogInformation("{UseCase} - Inserted user successfully; Name: {Email}",
                nameof(InsertUserUseCase), command.Email);

            output.AddResult(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{UseCase} - An unexpected error has occurred;",
                nameof(InsertUserUseCase));

            output.AddErrorMessage($"An unexpected error occurred while inserting the user");
        }
        return output;
    }
}