using FlightWatch.Application.Common;
using MediatR;

namespace FlightWatch.Application.Features.Auth.Commands.RevokeToken;

public record RevokeTokenCommand(
    string RefreshToken,
    string IpAddress) : IRequest<Result>;

