using Application.UseCases.Users.SearchUserById.Commands;
using FluentValidation;

namespace Application.UseCases.Users.SearchUserById.Validations;

public class SearchUserByIdCommandValidator : AbstractValidator<SearchUserByIdCommand>
{
    public SearchUserByIdCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
    }
}