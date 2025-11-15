using FlightWatch.Domain.Entities;
using System.Security.Claims;

namespace FlightWatch.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}

