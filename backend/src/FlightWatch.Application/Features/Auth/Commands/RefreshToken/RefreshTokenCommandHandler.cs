using AutoMapper;
using FlightWatch.Application.Common;
using FlightWatch.Application.Configuration;
using FlightWatch.Application.DTOs.Auth;
using FlightWatch.Application.Interfaces;
using FlightWatch.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlightWatch.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ITokenService tokenService,
    IOptions<JwtSettings> jwtSettings,
    IMapper mapper,
    ILogger<RefreshTokenCommandHandler> logger) : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly ITokenService _tokenService = tokenService;
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<RefreshTokenCommandHandler> _logger = logger;

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (token == null || !token.IsActive)
        {
            _logger.LogWarning("Invalid refresh token attempt from IP: {IpAddress}", request.IpAddress);
            return Result.Failure<AuthResponse>(
                Error.Validation("INVALID_TOKEN", "Invalid or expired refresh token"));
        }

        var user = await _userRepository.GetByIdAsync(token.UserId);

        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("Refresh token for inactive/non-existent user: {UserId}", token.UserId);
            return Result.Failure<AuthResponse>(
                Error.Validation("USER_NOT_FOUND", "User not found or inactive"));
        }

        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = request.IpAddress;

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        token.ReplacedByToken = newRefreshToken;

        await _refreshTokenRepository.UpdateAsync(token);

        _logger.LogInformation("Refresh token rotated for user: {UserId}", user.Id);

        var authResponse = await GenerateAuthResponseAsync(user, request.IpAddress, newRefreshToken);

        return Result.Success(authResponse);
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user, string ipAddress, string refreshToken)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);

        var refreshTokenEntity = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedByIp = ipAddress
        };

        await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            User = _mapper.Map<UserDto>(user)
        };
    }
}

