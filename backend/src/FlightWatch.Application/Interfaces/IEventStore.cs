using FlightWatch.Domain.Events;

namespace FlightWatch.Application.Interfaces;

public interface IEventStore
{
    Task SaveEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
    
    Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<IDomainEvent>> GetEventsByTypeAsync(Type eventType, CancellationToken cancellationToken = default);
}



