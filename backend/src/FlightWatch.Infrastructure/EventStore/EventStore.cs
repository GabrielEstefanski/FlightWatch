<<<<<<< HEAD
using FlightWatch.Application.Interfaces;
using FlightWatch.Domain.Events;
using FlightWatch.Infrastructure.Data;
using MongoDB.Driver;

namespace FlightWatch.Infrastructure.EventStore;

public class EventStore : IEventStore
{
    private readonly IMongoCollection<MongoDbContext.StoredEvent> _collection;

    public EventStore(MongoDbContext context)
    {
        _collection = context.Events;
    }

    public async Task SaveEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IDomainEvent
    {
        var storedEvent = new MongoDbContext.StoredEvent
        {
            EventId = @event.EventId,
            EventType = @event.GetType().Name,
            OccurredOn = @event.OccurredOn,
            EventData = @event
        };

        await _collection.InsertOneAsync(storedEvent, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId, CancellationToken cancellationToken = default)
    {
        var events = await _collection
            .Find(e => e.EventData is IDomainEvent)
            .ToListAsync(cancellationToken);

        return events
            .Select(e => e.EventData)
            .Cast<IDomainEvent>();
    }

    public async Task<IEnumerable<IDomainEvent>> GetEventsByTypeAsync(Type eventType, CancellationToken cancellationToken = default)
    {
        var events = await _collection
            .Find(e => e.EventType == eventType.Name)
            .ToListAsync(cancellationToken);

        return events
            .Select(e => e.EventData)
            .Cast<IDomainEvent>();
    }
}

=======
using FlightWatch.Application.Interfaces;
using FlightWatch.Domain.Events;
using FlightWatch.Infrastructure.Data;
using MongoDB.Driver;

namespace FlightWatch.Infrastructure.EventStore;

public class EventStore : IEventStore
{
    private readonly IMongoCollection<MongoDbContext.StoredEvent> _collection;

    public EventStore(MongoDbContext context)
    {
        _collection = context.Events;
    }

    public async Task SaveEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IDomainEvent
    {
        var storedEvent = new MongoDbContext.StoredEvent
        {
            EventId = @event.EventId,
            EventType = @event.GetType().Name,
            OccurredOn = @event.OccurredOn,
            EventData = @event
        };

        await _collection.InsertOneAsync(storedEvent, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId, CancellationToken cancellationToken = default)
    {
        var events = await _collection
            .Find(e => e.EventData is IDomainEvent)
            .ToListAsync(cancellationToken);

        return events
            .Select(e => e.EventData)
            .Cast<IDomainEvent>();
    }

    public async Task<IEnumerable<IDomainEvent>> GetEventsByTypeAsync(Type eventType, CancellationToken cancellationToken = default)
    {
        var events = await _collection
            .Find(e => e.EventType == eventType.Name)
            .ToListAsync(cancellationToken);

        return events
            .Select(e => e.EventData)
            .Cast<IDomainEvent>();
    }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
