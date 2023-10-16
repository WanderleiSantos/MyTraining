using Application.Shared.Extensions;
using Application.Shared.Models;
using Application.UseCases.Users.SearchUserById.Commands;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Application.Shared.Mappers;
using Core.Interfaces.Persistence.Repositories;
using Core.Shared.Errors;
using Errors = Core.Shared.Errors.Errors;

namespace Application.UseCases.Users.SearchUserById;

public class SearchUserByIdUseCase : ISearchUserByIdUseCase
{
    private readonly ILogger<SearchUserByIdUseCase> _logger;
    private readonly IUserRepository _repository;
    private readonly IValidator<SearchUserByIdCommand> _validator;

    public SearchUserByIdUseCase(ILogger<SearchUserByIdUseCase> logger, IUserRepository repository, IValidator<SearchUserByIdCommand> validator)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Output> ExecuteAsync(SearchUserByIdCommand command, CancellationToken cancellationToken)
    {
        var output = new Output();
        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            
            output.AddValidationResult(validationResult);

            if (!output.IsValid)
                return output;

            _logger.LogInformation("{UseCase} - Search user by id: {id}", nameof(SearchUserByIdUseCase), command.Id);

            var user = await _repository.GetByIdAsync(command.Id, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("{UseCase} - User does not exist; Id: {id}", nameof(SearchUserByIdUseCase), command.Id);
                
                output.AddError(Errors.User.DoesNotExist);
                return output;
            }

            _logger.LogInformation("{UseCase} - Search user finish successfully; Id: {id}", nameof(SearchUserByIdUseCase), command.Id);

            output.AddResult(user.MapToResponse());
        }
        catch (Exception e)
        {
            _logger.LogError(e,"{UseCase} -  An unexpected error has occurred;", nameof(SearchUserByIdUseCase));
            
            output.AddError(Error.Unexpected());
        }

        return output;
    }
}