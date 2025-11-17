using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs.FlightMonitoring;
using MediatR;

namespace FlightWatch.Application.Features.FlightMonitoring.Commands.Subscribe;

public record SubscribeFlightCommand(
    string ConnectionId,
    string? AreaName,
    double MinLatitude,
    double MaxLatitude,
    double MinLongitude,
    double MaxLongitude,
    int UpdateIntervalSeconds,
    Guid? UserId) : IRequest<Result<SubscriptionDto>>;
