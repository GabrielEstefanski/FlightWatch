using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs.Auth;
using MediatR;

namespace FlightWatch.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName,
    string IpAddress) : IRequest<Result<AuthResponse>>;

