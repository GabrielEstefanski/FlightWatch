<<<<<<< HEAD
using FlightWatch.Domain.Events;

namespace FlightWatch.Application.Interfaces;

public interface IEventStore
{
    Task SaveEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
    
    Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<IDomainEvent>> GetEventsByTypeAsync(Type eventType, CancellationToken cancellationToken = default);
}



=======
using FlightWatch.Domain.Events;

namespace FlightWatch.Application.Interfaces;

public interface IEventStore
{
    Task SaveEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
    
    Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<IDomainEvent>> GetEventsByTypeAsync(Type eventType, CancellationToken cancellationToken = default);
}



>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
