<<<<<<< HEAD
using FlightWatch.Application.Events;

namespace FlightWatch.Application.Interfaces;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
    
    Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);
}



=======
using FlightWatch.Application.Events;

namespace FlightWatch.Application.Interfaces;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
    
    Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);
}



>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
