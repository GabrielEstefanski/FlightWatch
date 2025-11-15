<<<<<<< HEAD
namespace FlightWatch.Application.Events;

public abstract class IntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}



=======
namespace FlightWatch.Application.Events;

public abstract class IntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}



>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
