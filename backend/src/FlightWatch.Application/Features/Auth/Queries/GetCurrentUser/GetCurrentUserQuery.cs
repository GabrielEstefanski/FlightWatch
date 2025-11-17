using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs.Auth;
using MediatR;

namespace FlightWatch.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IRequest<Result<UserDto>>;
