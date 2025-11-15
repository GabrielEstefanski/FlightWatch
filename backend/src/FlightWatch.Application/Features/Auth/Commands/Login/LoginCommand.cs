using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs.Auth;
using MediatR;

namespace FlightWatch.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password,
    string IpAddress) : IRequest<Result<AuthResponse>>;

