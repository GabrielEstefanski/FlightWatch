<<<<<<< HEAD
using FlightWatch.Application.Common;
using FlightWatch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Application.Features.FlightMonitoring.Commands.Unsubscribe;

public class UnsubscribeFlightCommandHandler(
    IFlightSubscriptionRepository subscriptionRepository,
    ILogger<UnsubscribeFlightCommandHandler> logger) : IRequestHandler<UnsubscribeFlightCommand, Result>
{
    private readonly IFlightSubscriptionRepository _subscriptionRepository = subscriptionRepository;
    private readonly ILogger<UnsubscribeFlightCommandHandler> _logger = logger;

    public async Task<Result> Handle(UnsubscribeFlightCommand request, CancellationToken cancellationToken)
    {
        await _subscriptionRepository.DeleteByConnectionIdAsync(request.ConnectionId);

        _logger.LogInformation(
            "Flight subscription removed. ConnectionId: {ConnectionId}",
            request.ConnectionId);

        return Result.Success();
    }
}

=======
using FlightWatch.Application.Common;
using FlightWatch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Application.Features.FlightMonitoring.Commands.Unsubscribe;

public class UnsubscribeFlightCommandHandler(
    IFlightSubscriptionRepository subscriptionRepository,
    ILogger<UnsubscribeFlightCommandHandler> logger) : IRequestHandler<UnsubscribeFlightCommand, Result>
{
    private readonly IFlightSubscriptionRepository _subscriptionRepository = subscriptionRepository;
    private readonly ILogger<UnsubscribeFlightCommandHandler> _logger = logger;

    public async Task<Result> Handle(UnsubscribeFlightCommand request, CancellationToken cancellationToken)
    {
        await _subscriptionRepository.DeleteByConnectionIdAsync(request.ConnectionId);

        _logger.LogInformation(
            "Flight subscription removed. ConnectionId: {ConnectionId}",
            request.ConnectionId);

        return Result.Success();
    }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
