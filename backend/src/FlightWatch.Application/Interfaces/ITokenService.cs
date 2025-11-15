<<<<<<< HEAD
using FlightWatch.Domain.Entities;
using System.Security.Claims;

namespace FlightWatch.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}

=======
using FlightWatch.Domain.Entities;
using System.Security.Claims;

namespace FlightWatch.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
