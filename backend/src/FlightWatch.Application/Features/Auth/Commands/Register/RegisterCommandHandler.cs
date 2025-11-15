using AutoMapper;
using FlightWatch.Application.Common;
using FlightWatch.Application.Configuration;
using FlightWatch.Application.DTOs.Auth;
using FlightWatch.Application.Interfaces;
using FlightWatch.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlightWatch.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IOptions<JwtSettings> jwtSettings,
    IMapper mapper,
    ILogger<RegisterCommandHandler> logger) : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenService _tokenService = tokenService;
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<RegisterCommandHandler> _logger = logger;

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsAsync(request.Email))
        {
            _logger.LogWarning("Registration attempt with existing email: {Email}", request.Email);
            return Result.Failure<AuthResponse>(
                Error.Validation("EMAIL_EXISTS", "Email already registered"));
        }

        var user = new User
        {
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Roles = ["User"],
            IsActive = true,
            IsEmailConfirmed = false
        };

        user = await _userRepository.CreateAsync(user);

        _logger.LogInformation("User registered successfully: {UserId}, {Email}", user.Id, user.Email);

        var authResponse = await GenerateAuthResponseAsync(user, request.IpAddress);

        return Result.Success(authResponse);
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user, string ipAddress)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

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

