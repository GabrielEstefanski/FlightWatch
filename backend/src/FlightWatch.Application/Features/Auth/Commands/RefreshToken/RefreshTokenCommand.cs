<<<<<<< HEAD
using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs.Auth;
using MediatR;

namespace FlightWatch.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken,
    string IpAddress) : IRequest<Result<AuthResponse>>;

=======
using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs.Auth;
using MediatR;

namespace FlightWatch.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken,
    string IpAddress) : IRequest<Result<AuthResponse>>;

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
