namespace FlightWatch.Application.Events;

public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTime CreatedAt { get; }
}



