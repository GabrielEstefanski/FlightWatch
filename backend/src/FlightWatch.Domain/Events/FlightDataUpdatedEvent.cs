<<<<<<< HEAD
namespace FlightWatch.Domain.Events;

public class FlightDataUpdatedEvent : DomainEvent
{
    public Guid SubscriptionId { get; init; }
    public int FlightCount { get; init; }
    public List<string> FlightNumbers { get; init; } = [];
    public string AreaName { get; init; } = string.Empty;
}



=======
namespace FlightWatch.Domain.Events;

public class FlightDataUpdatedEvent : DomainEvent
{
    public Guid SubscriptionId { get; init; }
    public int FlightCount { get; init; }
    public List<string> FlightNumbers { get; init; } = [];
    public string AreaName { get; init; } = string.Empty;
}



>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
