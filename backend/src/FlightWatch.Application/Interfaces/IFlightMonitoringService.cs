using FlightWatch.Application.Common;

namespace FlightWatch.Application.Interfaces;

public interface IFlightMonitoringService
{
    Task<Result> UnsubscribeAsync(string connectionId);
    Task<Result> UpdateFlightsForSubscriptionAsync(Guid subscriptionId);
}

