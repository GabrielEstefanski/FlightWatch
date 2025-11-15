namespace FlightWatch.Application.Events;

public class FlightSubscriptionCreatedIntegrationEvent : IntegrationEvent
{
    public Guid SubscriptionId { get; init; }
    public string ConnectionId { get; init; } = string.Empty;
    public Guid? UserId { get; init; }
    public string AreaName { get; init; } = string.Empty;
    public double MinLatitude { get; init; }
    public double MaxLatitude { get; init; }
    public double MinLongitude { get; init; }
    public double MaxLongitude { get; init; }
}



