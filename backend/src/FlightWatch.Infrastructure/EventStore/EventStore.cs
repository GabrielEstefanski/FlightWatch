using FlightWatch.Application.Interfaces;
using FlightWatch.Domain.Events;
using FlightWatch.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace FlightWatch.Infrastructure.EventStore;

public class EventStore(MongoDbContext context) : IEventStore
{
    private readonly IMongoCollection<MongoDbContext.StoredEvent> _collection = context.Events;

    public async Task SaveEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IDomainEvent
    {
        var eventDocument = new BsonDocument();
        BsonSerializer.Serialize(new BsonDocumentWriter(eventDocument), @event);
        
        var storedEvent = new MongoDbContext.StoredEvent
        {
            EventId = @event.EventId,
            EventType = @event.GetType().FullName ?? @event.GetType().Name,
            OccurredOn = @event.OccurredOn,
            EventData = eventDocument
        };

        await _collection.InsertOneAsync(storedEvent, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId, CancellationToken cancellationToken = default)
    {
        var events = await _collection
            .Find(e => e.EventId == aggregateId)
            .ToListAsync(cancellationToken);

        return events.Select(DeserializeEvent);
    }

    public async Task<IEnumerable<IDomainEvent>> GetEventsByTypeAsync(Type eventType, CancellationToken cancellationToken = default)
    {
        var events = await _collection
            .Find(e => e.EventType == eventType.FullName || e.EventType == eventType.Name)
            .ToListAsync(cancellationToken);

        return events.Select(DeserializeEvent);
    }

    private IDomainEvent DeserializeEvent(MongoDbContext.StoredEvent storedEvent)
    {
        var eventType = (Type.GetType(storedEvent.EventType) 
            ?? AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == storedEvent.EventType && typeof(IDomainEvent).IsAssignableFrom(t)))
                    ?? throw new InvalidOperationException($"Event type '{storedEvent.EventType}' not found.");

        return (IDomainEvent)BsonSerializer.Deserialize(storedEvent.EventData, eventType);
    }
}

