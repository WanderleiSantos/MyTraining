using Application.Shared.Models;
using Application.UseCases.Auth.RefreshToken.Commands;

namespace Application.UseCases.Auth.RefreshToken;

public interface IRefreshTokenUseCase
{
    Task<Output> ExecuteAsync(RefreshTokenCommand command, CancellationToken cancellationToken);
}