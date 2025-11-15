<<<<<<< HEAD
namespace FlightWatch.Domain.Events;

public class FlightSubscriptionCreatedEvent : DomainEvent
{
    public Guid SubscriptionId { get; init; }
    public string ConnectionId { get; init; } = string.Empty;
    public Guid? UserId { get; init; }
    public string AreaName { get; init; } = string.Empty;
    public double MinLatitude { get; init; }
    public double MaxLatitude { get; init; }
    public double MinLongitude { get; init; }
    public double MaxLongitude { get; init; }
    public int UpdateIntervalSeconds { get; init; }
}



=======
namespace FlightWatch.Domain.Events;

public class FlightSubscriptionCreatedEvent : DomainEvent
{
    public Guid SubscriptionId { get; init; }
    public string ConnectionId { get; init; } = string.Empty;
    public Guid? UserId { get; init; }
    public string AreaName { get; init; } = string.Empty;
    public double MinLatitude { get; init; }
    public double MaxLatitude { get; init; }
    public double MinLongitude { get; init; }
    public double MaxLongitude { get; init; }
    public int UpdateIntervalSeconds { get; init; }
}



>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
