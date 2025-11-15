<<<<<<< HEAD
using FlightWatch.Application.DTOs;
using FlightWatch.Application.Events;
using FlightWatch.Application.Helpers;
using FlightWatch.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Infrastructure.EventHandlers;

public class FlightDataUpdatedEventHandler(
    ILogger<FlightDataUpdatedEventHandler> logger,
    IFlightNotificationService notificationService) : IConsumer<FlightDataUpdatedIntegrationEvent>
{
    private readonly ILogger<FlightDataUpdatedEventHandler> _logger = logger;
    private readonly IFlightNotificationService _notificationService = notificationService;

    public async Task Consume(ConsumeContext<FlightDataUpdatedIntegrationEvent> context)
    {
        var @event = context.Message;

        _logger.LogInformation(
            "Processing FlightDataUpdatedEvent for SubscriptionId: {SubscriptionId}, FlightCount: {FlightCount}",
            @event.SubscriptionId,
            @event.FlightCount);

        try
        {
            var flightDtos = @event.Flights.Select(f => new FlightDto
            {
                FlightNumber = f.FlightNumber,
                Airline = f.Airline,
                Latitude = f.Latitude,
                Longitude = f.Longitude,
                Origin = f.Origin,
                Destination = f.Destination,
                Altitude = f.Altitude,
                Velocity = f.Velocity,
                Direction = f.Direction,
                FlightStatus = f.FlightStatus,
                IsLive = f.IsLive,
                Category = f.Category,
                CategoryDescription = AircraftCategoryHelper.GetCategoryShort(f.Category)
            }).ToList();

            await _notificationService.NotifyFlightUpdatesAsync(
                @event.ConnectionId,
                @event.SubscriptionId,
                flightDtos);

            _logger.LogInformation(
                "Flight data update for subscription {SubscriptionId} successfully sent to connection {ConnectionId}",
                @event.SubscriptionId,
                @event.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing FlightDataUpdatedEvent for SubscriptionId: {SubscriptionId}",
                @event.SubscriptionId);
            throw;
        }
    }
}

=======
using FlightWatch.Application.DTOs;
using FlightWatch.Application.Events;
using FlightWatch.Application.Helpers;
using FlightWatch.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Infrastructure.EventHandlers;

public class FlightDataUpdatedEventHandler(
    ILogger<FlightDataUpdatedEventHandler> logger,
    IFlightNotificationService notificationService) : IConsumer<FlightDataUpdatedIntegrationEvent>
{
    private readonly ILogger<FlightDataUpdatedEventHandler> _logger = logger;
    private readonly IFlightNotificationService _notificationService = notificationService;

    public async Task Consume(ConsumeContext<FlightDataUpdatedIntegrationEvent> context)
    {
        var @event = context.Message;

        _logger.LogInformation(
            "Processing FlightDataUpdatedEvent for SubscriptionId: {SubscriptionId}, FlightCount: {FlightCount}",
            @event.SubscriptionId,
            @event.FlightCount);

        try
        {
            var flightDtos = @event.Flights.Select(f => new FlightDto
            {
                FlightNumber = f.FlightNumber,
                Airline = f.Airline,
                Latitude = f.Latitude,
                Longitude = f.Longitude,
                Origin = f.Origin,
                Destination = f.Destination,
                Altitude = f.Altitude,
                Velocity = f.Velocity,
                Direction = f.Direction,
                FlightStatus = f.FlightStatus,
                IsLive = f.IsLive,
                Category = f.Category,
                CategoryDescription = AircraftCategoryHelper.GetCategoryShort(f.Category)
            }).ToList();

            await _notificationService.NotifyFlightUpdatesAsync(
                @event.ConnectionId,
                @event.SubscriptionId,
                flightDtos);

            _logger.LogInformation(
                "Flight data update for subscription {SubscriptionId} successfully sent to connection {ConnectionId}",
                @event.SubscriptionId,
                @event.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing FlightDataUpdatedEvent for SubscriptionId: {SubscriptionId}",
                @event.SubscriptionId);
            throw;
        }
    }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
