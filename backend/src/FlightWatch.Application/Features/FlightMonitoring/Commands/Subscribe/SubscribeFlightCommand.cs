<<<<<<< HEAD
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

=======
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

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
