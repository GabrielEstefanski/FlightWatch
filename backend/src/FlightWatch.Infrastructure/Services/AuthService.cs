using AutoMapper;
using FlightWatch.Application.Common;
using FlightWatch.Application.Configuration;
using FlightWatch.Application.DTOs.Auth;
using FlightWatch.Application.Interfaces;
using FlightWatch.Domain.Entities;
using FlightWatch.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlightWatch.Infrastructure.Services;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IOptions<JwtSettings> jwtSettings,
    IMapper mapper,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenService _tokenService = tokenService;
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, string ipAddress)
    {
        try
        {
            if (await _userRepository.ExistsAsync(request.Email))
            {
                _logger.LogWarning("Registration attempt with existing email: {Email}", request.Email);
                return Result.Failure<AuthResponse>(
                    Error.Validation("EMAIL_EXISTS", "Email already registered"));
            }

            var user = _mapper.Map<User>(request);
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);

            user = await _userRepository.CreateAsync(user);

            _logger.LogInformation("User registered successfully: {UserId}, {Email}", user.Id, user.Email);

            var authResponse = await GenerateAuthResponseAsync(user, ipAddress);

            return Result.Success(authResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for email: {Email}", request.Email);
            return Result.Failure<AuthResponse>(
                Error.Failure("REGISTRATION_ERROR", "An error occurred during registration"));
        }
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, string ipAddress)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
                return Result.Failure<AuthResponse>(
                    Error.Validation("INVALID_CREDENTIALS", "Invalid email or password"));
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt for inactive user: {Email}", request.Email);
                return Result.Failure<AuthResponse>(
                    Error.Validation("ACCOUNT_INACTIVE", "Account is inactive"));
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User logged in successfully: {UserId}, {Email}", user.Id, user.Email);

            var authResponse = await GenerateAuthResponseAsync(user, ipAddress);

            return Result.Success(authResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return Result.Failure<AuthResponse>(
                Error.Failure("LOGIN_ERROR", "An error occurred during login"));
        }
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken, string ipAddress)
    {
        try
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token == null || !token.IsActive)
            {
                _logger.LogWarning("Invalid refresh token attempt from IP: {IpAddress}", ipAddress);
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
            token.RevokedByIp = ipAddress;
            
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            token.ReplacedByToken = newRefreshToken;
            
            await _refreshTokenRepository.UpdateAsync(token);

            _logger.LogInformation("Refresh token rotated for user: {UserId}", user.Id);

            var authResponse = await GenerateAuthResponseAsync(user, ipAddress, newRefreshToken);

            return Result.Success(authResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh from IP: {IpAddress}", ipAddress);
            return Result.Failure<AuthResponse>(
                Error.Failure("REFRESH_ERROR", "An error occurred during token refresh"));
        }
    }

    public async Task<Result> RevokeTokenAsync(string refreshToken, string ipAddress)
    {
        try
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token == null || !token.IsActive)
            {
                _logger.LogWarning("Revoke attempt for invalid token from IP: {IpAddress}", ipAddress);
                return Result.Failure(
                    Error.Validation("INVALID_TOKEN", "Invalid or already revoked token"));
            }

            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            await _refreshTokenRepository.UpdateAsync(token);

            _logger.LogInformation("Refresh token revoked for user: {UserId}", token.UserId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token revocation from IP: {IpAddress}", ipAddress);
            return Result.Failure(
                Error.Failure("REVOKE_ERROR", "An error occurred during token revocation"));
        }
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return Result.Failure<UserDto>(
                    Error.NotFound("USER_NOT_FOUND", "User not found"));
            }

            return Result.Success(_mapper.Map<UserDto>(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user: {UserId}", userId);
            return Result.Failure<UserDto>(
                Error.Failure("USER_RETRIEVAL_ERROR", "An error occurred while retrieving user"));
        }
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user, string ipAddress, string? refreshToken = null)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        refreshToken ??= _tokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
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

