
using Application.UseCases.Auth.RefreshToken.Commands;
using FluentValidation;

namespace Application.UseCases.Auth.RefreshToken.Validations;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}