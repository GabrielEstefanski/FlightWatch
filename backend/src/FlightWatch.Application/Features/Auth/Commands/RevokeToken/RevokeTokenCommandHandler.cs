using FlightWatch.Application.Common;
using FlightWatch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Application.Features.Auth.Commands.RevokeToken;

public class RevokeTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    ILogger<RevokeTokenCommandHandler> logger) : IRequestHandler<RevokeTokenCommand, Result>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly ILogger<RevokeTokenCommandHandler> _logger = logger;

    public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (token == null || !token.IsActive)
        {
            _logger.LogWarning("Revoke attempt for invalid token from IP: {IpAddress}", request.IpAddress);
            return Result.Failure(
                Error.Validation("INVALID_TOKEN", "Invalid or already revoked token"));
        }

        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = request.IpAddress;
        await _refreshTokenRepository.UpdateAsync(token);

        _logger.LogInformation("Refresh token revoked for user: {UserId}", token.UserId);

        return Result.Success();
    }
}

