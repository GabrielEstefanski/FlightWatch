using AutoMapper;
using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs;
using FlightWatch.Application.DTOs.FlightMonitoring;
using FlightWatch.Application.Events;
using FlightWatch.Application.Interfaces;
using FlightWatch.Domain.Entities;
using FlightWatch.Domain.Events;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Infrastructure.Services;

public class FlightMonitoringService(
    IFlightSubscriptionRepository subscriptionRepository,
    IOpenSkyClient openSkyClient,
    IFlightNotificationService notificationService,
    IEventBus eventBus,
    IEventStore eventStore,
    IMapper mapper,
    ILogger<FlightMonitoringService> logger) : IFlightMonitoringService
{
    private readonly IFlightSubscriptionRepository _subscriptionRepository = subscriptionRepository;
    private readonly IOpenSkyClient _openSkyClient = openSkyClient;
    private readonly IFlightNotificationService _notificationService = notificationService;
    private readonly IEventBus _eventBus = eventBus;
    private readonly IEventStore _eventStore = eventStore;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<FlightMonitoringService> _logger = logger;

    public async Task<Result> UnsubscribeAsync(string connectionId)
    {
        try
        {
            await _subscriptionRepository.DeleteByConnectionIdAsync(connectionId);

            _logger.LogInformation(
                "Flight subscription removed. ConnectionId: {ConnectionId}",
                connectionId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error removing flight subscription. ConnectionId: {ConnectionId}",
                connectionId);

            return Result.Failure(
                Error.Failure("UNSUBSCRIBE_ERROR", "Failed to remove subscription"));
        }
    }

    public async Task<Result> UpdateFlightsForSubscriptionAsync(Guid subscriptionId)
    {
        try
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId);

            if (subscription == null || !subscription.IsActive)
            {
                return Result.Failure(
                    Error.NotFound("SUBSCRIPTION_NOT_FOUND", "Subscription not found or inactive"));
            }

            var flightsResult = await _openSkyClient.GetFlightsByBoundingBoxAsync(
                subscription.MinLatitude,
                subscription.MaxLatitude,
                subscription.MinLongitude,
                subscription.MaxLongitude);

            if (flightsResult.IsFailure)
            {
                _logger.LogWarning(
                    "Failed to fetch flights for subscription. SubscriptionId: {SubscriptionId}, Error: {Error}",
                    subscriptionId,
                    flightsResult.Error.Message);

                await _notificationService.SendErrorAsync(
                    subscription.ConnectionId,
                    flightsResult.Error.Message);

                return Result.Failure(flightsResult.Error);
            }

            var flights = flightsResult.Value.ToList();

            _logger.LogInformation(
                "Retrieved {FlightCount} flights for area: {AreaName}",
                flights.Count,
                subscription.AreaName);

            var domainEvent = new FlightDataUpdatedEvent
            {
                SubscriptionId = subscription.Id,
                FlightCount = flights.Count,
                FlightNumbers = [.. flights.Select(f => f.FlightNumber)],
                AreaName = subscription.AreaName
            };

            await _eventStore.SaveEventAsync(domainEvent);

            var integrationEvent = new FlightDataUpdatedIntegrationEvent
            {
                SubscriptionId = subscription.Id,
                ConnectionId = subscription.ConnectionId,
                FlightCount = flights.Count,
                AreaName = subscription.AreaName,
                Flights = [.. flights.Select(f => new FlightData
                {
                    FlightNumber = f.FlightNumber,
                    Airline = f.Airline,
                    Latitude = f.Latitude ?? 0,
                    Longitude = f.Longitude ?? 0,
                    Origin = f.Origin,
                    Destination = f.Destination,
                    Altitude = f.Altitude,
                    Velocity = f.Velocity,
                    Direction = f.Direction,
                    FlightStatus = f.FlightStatus,
                    IsLive = f.IsLive,
                    Category = f.Category
                })]
            };

            await _eventBus.PublishAsync(integrationEvent);

            subscription.LastUpdatedAt = DateTime.UtcNow;
            await _subscriptionRepository.UpdateAsync(subscription);

            _logger.LogInformation(
                "Flight update event published. SubscriptionId: {SubscriptionId}, FlightCount: {FlightCount}",
                subscriptionId,
                flights.Count);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error updating flights for subscription. SubscriptionId: {SubscriptionId}",
                subscriptionId);

            return Result.Failure(
                Error.Failure("UPDATE_ERROR", "Failed to update flights"));
        }
    }

}

