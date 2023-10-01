using FluentValidation;
using MyTraining.Application.UseCases.Users.SearchUserById.Commands;

namespace MyTraining.Application.UseCases.Users.SearchUserById.Validations;

public class SearchUserByIdValidator : AbstractValidator<SearchUserByIdCommand>
{
    public SearchUserByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}