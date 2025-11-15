using FlightWatch.Application.Events;
using FlightWatch.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Infrastructure.EventHandlers;

public class FlightSubscriptionCreatedEventHandler(
    ILogger<FlightSubscriptionCreatedEventHandler> logger,
    IFlightNotificationService notificationService) : IConsumer<FlightSubscriptionCreatedIntegrationEvent>
{
    private readonly ILogger<FlightSubscriptionCreatedEventHandler> _logger = logger;
    private readonly IFlightNotificationService _notificationService = notificationService;

    public async Task Consume(ConsumeContext<FlightSubscriptionCreatedIntegrationEvent> context)
    {
        var @event = context.Message;

        _logger.LogInformation(
            "Processing FlightSubscriptionCreatedEvent for SubscriptionId: {SubscriptionId}, Area: {AreaName}",
            @event.SubscriptionId,
            @event.AreaName);

        try
        {
            await _notificationService.NotifySubscriptionCreatedAsync(
                @event.ConnectionId,
                @event.SubscriptionId,
                @event.AreaName);

            _logger.LogInformation(
                "Flight subscription {SubscriptionId} successfully processed and client {ConnectionId} notified",
                @event.SubscriptionId,
                @event.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing FlightSubscriptionCreatedEvent for SubscriptionId: {SubscriptionId}",
                @event.SubscriptionId);
            throw;
        }
    }
}

