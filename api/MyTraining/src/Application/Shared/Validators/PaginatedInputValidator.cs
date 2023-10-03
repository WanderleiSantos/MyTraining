using Application.Shared.Models;
using FluentValidation;

namespace Application.Shared.Validators;

public abstract class PaginatedInputValidator<T> : AbstractValidator<T> where T : PaginatedInput
{
    protected PaginatedInputValidator()
    {
        RuleFor(x => x.PageSize)
            .LessThanOrEqualTo(100)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);
    }
}