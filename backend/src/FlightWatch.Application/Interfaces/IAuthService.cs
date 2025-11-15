using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs.Auth;

namespace FlightWatch.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, string ipAddress);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, string ipAddress);
    Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken, string ipAddress);
    Task<Result> RevokeTokenAsync(string refreshToken, string ipAddress);
    Task<Result<UserDto>> GetUserByIdAsync(Guid userId);
}

