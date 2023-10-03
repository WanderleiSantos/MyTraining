using FluentValidation;
using MyTraining.Application.UseCases.Users.SearchUserById.Commands;

namespace MyTraining.Application.UseCases.Users.SearchUserById.Validations;

public class SearchUserByIdCommandValidator : AbstractValidator<SearchUserByIdCommand>
{
    public SearchUserByIdCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}