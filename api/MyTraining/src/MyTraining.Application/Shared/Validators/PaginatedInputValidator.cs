using FluentValidation;
using MyTraining.Application.Shared.Models;

namespace MyTraining.Application.Shared.Validators;

public class PaginatedInputValidator<T> : AbstractValidator<T> where T : PaginatedInput
{
    public PaginatedInputValidator()
    {
        RuleFor(x => x.PageSize)
            .LessThanOrEqualTo(100)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);
    }
}