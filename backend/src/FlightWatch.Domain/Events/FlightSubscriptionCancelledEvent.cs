namespace FlightWatch.Domain.Events;

public class FlightSubscriptionCancelledEvent : DomainEvent
{
    public Guid SubscriptionId { get; init; }
    public string ConnectionId { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}



