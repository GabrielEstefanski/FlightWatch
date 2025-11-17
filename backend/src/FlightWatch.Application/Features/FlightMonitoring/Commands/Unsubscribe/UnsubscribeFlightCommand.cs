using FlightWatch.Application.Common;
using MediatR;

namespace FlightWatch.Application.Features.FlightMonitoring.Commands.Unsubscribe;

public record UnsubscribeFlightCommand(string ConnectionId) : IRequest<Result>;

