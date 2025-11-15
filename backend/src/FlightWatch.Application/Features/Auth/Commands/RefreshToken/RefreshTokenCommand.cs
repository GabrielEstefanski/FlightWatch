using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs.Auth;
using MediatR;

namespace FlightWatch.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken,
    string IpAddress) : IRequest<Result<AuthResponse>>;

