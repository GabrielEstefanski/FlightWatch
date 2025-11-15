namespace FlightWatch.Domain.Events;

public class FlightDataUpdatedEvent : DomainEvent
{
    public Guid SubscriptionId { get; init; }
    public int FlightCount { get; init; }
    public List<string> FlightNumbers { get; init; } = [];
    public string AreaName { get; init; } = string.Empty;
}



