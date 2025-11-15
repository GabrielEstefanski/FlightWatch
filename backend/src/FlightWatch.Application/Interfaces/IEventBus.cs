using FlightWatch.Application.Events;

namespace FlightWatch.Application.Interfaces;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
    
    Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);
}



