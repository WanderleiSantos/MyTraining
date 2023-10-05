using Application.Shared.Models;
using Application.UseCases.Users.SearchUserById.Commands;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Application.Shared.Mappers;
using Core.Interfaces.Persistence.Repositories;

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

            _logger.LogInformation("{UseCase} - Search user by id: {id}", 
                nameof(SearchUserByIdUseCase), command.Id);

            var result = await _repository.GetByIdAsync(command.Id, cancellationToken);

            if (result == null)
            {
                output.AddMessage("User does not exist");
                _logger.LogWarning("User does not exist");
                return output;
            }

            _logger.LogInformation("{UseCase} - Search user finish successfully, id: {id}",
                nameof(SearchUserByIdUseCase), command.Id);

            output.AddResult(result?.MapUserToSearchUserByIdResponse());
        }
        catch (Exception e)
        {
            _logger.LogError(e,"{UseCase} -  An unexpected error has occurred;", nameof(SearchUserByIdUseCase));
            output.AddErrorMessage("An unexpected error occurred while searching the user.");
        }

        return output;
    }
}