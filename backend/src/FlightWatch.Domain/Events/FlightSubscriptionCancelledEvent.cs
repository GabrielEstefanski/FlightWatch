<<<<<<< HEAD
namespace FlightWatch.Domain.Events;

public class FlightSubscriptionCancelledEvent : DomainEvent
{
    public Guid SubscriptionId { get; init; }
    public string ConnectionId { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}



=======
namespace FlightWatch.Domain.Events;

public class FlightSubscriptionCancelledEvent : DomainEvent
{
    public Guid SubscriptionId { get; init; }
    public string ConnectionId { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}



>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
