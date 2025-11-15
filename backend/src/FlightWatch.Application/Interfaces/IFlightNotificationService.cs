using FlightWatch.Application.DTOs;
using FlightWatch.Application.DTOs.FlightMonitoring;

namespace FlightWatch.Application.Interfaces;

public interface IFlightNotificationService
{
    Task SendFlightUpdateAsync(string connectionId, FlightUpdateDto update);
    Task SendErrorAsync(string connectionId, string errorMessage);
    Task NotifySubscriptionCreatedAsync(string connectionId, Guid subscriptionId, string areaName);
    Task NotifyFlightUpdatesAsync(string connectionId, Guid subscriptionId, List<FlightDto> flights);
}

