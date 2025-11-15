using FlightWatch.Api.Hubs;
using FlightWatch.Application.DTOs.FlightMonitoring;
using FlightWatch.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FlightWatch.Api.Services;

public class FlightNotificationService(
    IHubContext<FlightHub> hubContext,
    ILogger<FlightNotificationService> logger) : IFlightNotificationService
{
    private readonly IHubContext<FlightHub> _hubContext = hubContext;
    private readonly ILogger<FlightNotificationService> _logger = logger;

    public async Task SendFlightUpdateAsync(string connectionId, FlightUpdateDto update)
    {
        try
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("FlightUpdate", update);
            
            _logger.LogDebug(
                "Flight update sent. ConnectionId: {ConnectionId}, SubscriptionId: {SubscriptionId}, FlightCount: {FlightCount}",
                connectionId,
                update.SubscriptionId,
                update.FlightCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send flight update. ConnectionId: {ConnectionId}, SubscriptionId: {SubscriptionId}",
                connectionId,
                update.SubscriptionId);
        }
    }

    public async Task SendErrorAsync(string connectionId, string errorMessage)
    {
        try
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("Error", new
            {
                message = errorMessage,
                timestamp = DateTime.UtcNow
            });

            _logger.LogDebug(
                "Error notification sent. ConnectionId: {ConnectionId}, Message: {Message}",
                connectionId,
                errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send error notification. ConnectionId: {ConnectionId}",
                connectionId);
        }
    }

    public async Task NotifySubscriptionCreatedAsync(string connectionId, Guid subscriptionId, string areaName)
    {
        try
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("SubscriptionCreated", new
            {
                subscriptionId,
                areaName,
                timestamp = DateTime.UtcNow,
                message = $"Subscription created for area: {areaName}"
            });

            _logger.LogInformation(
                "Subscription created notification sent. ConnectionId: {ConnectionId}, SubscriptionId: {SubscriptionId}",
                connectionId,
                subscriptionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send subscription created notification. ConnectionId: {ConnectionId}",
                connectionId);
        }
    }

    public async Task NotifyFlightUpdatesAsync(string connectionId, Guid subscriptionId, List<Application.DTOs.FlightDto> flights)
    {
        try
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("FlightDataUpdated", new
            {
                subscriptionId,
                flightCount = flights.Count,
                flights,
                timestamp = DateTime.UtcNow
            });

            _logger.LogInformation(
                "Flight data updated notification sent. ConnectionId: {ConnectionId}, SubscriptionId: {SubscriptionId}, FlightCount: {FlightCount}",
                connectionId,
                subscriptionId,
                flights.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send flight data updated notification. ConnectionId: {ConnectionId}",
                connectionId);
        }
    }
}

