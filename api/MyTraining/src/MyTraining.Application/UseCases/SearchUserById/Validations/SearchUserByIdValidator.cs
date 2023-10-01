using FluentValidation;
using MyTraining.Application.UseCases.SearchUserById.Commands;

namespace MyTraining.Application.UseCases.SearchUserById.Validations;

public class SearchUserByIdValidator : AbstractValidator<SearchUserByIdCommand>
{
    public SearchUserByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}