<<<<<<< HEAD
using FlightWatch.Domain.Entities;

namespace FlightWatch.Application.Interfaces;

public interface IFlightSubscriptionRepository
{
    Task<FlightSubscription?> GetByIdAsync(Guid id);
    Task<FlightSubscription?> GetByConnectionIdAsync(string connectionId);
    Task<List<FlightSubscription>> GetActiveSubscriptionsAsync();
    Task<List<FlightSubscription>> GetByRouteAsync(string origin, string destination);
    Task<FlightSubscription> CreateAsync(FlightSubscription subscription);
    Task UpdateAsync(FlightSubscription subscription);
    Task DeleteAsync(Guid id);
    Task DeleteByConnectionIdAsync(string connectionId);
}

=======
using FlightWatch.Domain.Entities;

namespace FlightWatch.Application.Interfaces;

public interface IFlightSubscriptionRepository
{
    Task<FlightSubscription?> GetByIdAsync(Guid id);
    Task<FlightSubscription?> GetByConnectionIdAsync(string connectionId);
    Task<List<FlightSubscription>> GetActiveSubscriptionsAsync();
    Task<List<FlightSubscription>> GetByRouteAsync(string origin, string destination);
    Task<FlightSubscription> CreateAsync(FlightSubscription subscription);
    Task UpdateAsync(FlightSubscription subscription);
    Task DeleteAsync(Guid id);
    Task DeleteByConnectionIdAsync(string connectionId);
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
