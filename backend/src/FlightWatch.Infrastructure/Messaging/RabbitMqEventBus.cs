<<<<<<< HEAD
using FlightWatch.Application.Events;
using FlightWatch.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Infrastructure.Messaging;

public class RabbitMqEventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RabbitMqEventBus> _logger;

    public RabbitMqEventBus(IPublishEndpoint publishEndpoint, ILogger<RabbitMqEventBus> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IIntegrationEvent
    {
        try
        {
            _logger.LogInformation(
                "Publishing integration event: {EventType} with ID: {EventId}",
                typeof(TEvent).Name,
                @event.EventId);

            await _publishEndpoint.Publish(@event, cancellationToken);

            _logger.LogInformation(
                "Successfully published integration event: {EventType} with ID: {EventId}",
                typeof(TEvent).Name,
                @event.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error publishing integration event: {EventType} with ID: {EventId}",
                typeof(TEvent).Name,
                @event.EventId);
            throw;
        }
    }

    public async Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Publishing integration event: {EventType} with ID: {EventId}",
                @event.GetType().Name,
                @event.EventId);

            await _publishEndpoint.Publish(@event, @event.GetType(), cancellationToken);

            _logger.LogInformation(
                "Successfully published integration event: {EventType} with ID: {EventId}",
                @event.GetType().Name,
                @event.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error publishing integration event: {EventType} with ID: {EventId}",
                @event.GetType().Name,
                @event.EventId);
            throw;
        }
    }
}



=======
using FlightWatch.Application.Events;
using FlightWatch.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Infrastructure.Messaging;

public class RabbitMqEventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RabbitMqEventBus> _logger;

    public RabbitMqEventBus(IPublishEndpoint publishEndpoint, ILogger<RabbitMqEventBus> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IIntegrationEvent
    {
        try
        {
            _logger.LogInformation(
                "Publishing integration event: {EventType} with ID: {EventId}",
                typeof(TEvent).Name,
                @event.EventId);

            await _publishEndpoint.Publish(@event, cancellationToken);

            _logger.LogInformation(
                "Successfully published integration event: {EventType} with ID: {EventId}",
                typeof(TEvent).Name,
                @event.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error publishing integration event: {EventType} with ID: {EventId}",
                typeof(TEvent).Name,
                @event.EventId);
            throw;
        }
    }

    public async Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Publishing integration event: {EventType} with ID: {EventId}",
                @event.GetType().Name,
                @event.EventId);

            await _publishEndpoint.Publish(@event, @event.GetType(), cancellationToken);

            _logger.LogInformation(
                "Successfully published integration event: {EventType} with ID: {EventId}",
                @event.GetType().Name,
                @event.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error publishing integration event: {EventType} with ID: {EventId}",
                @event.GetType().Name,
                @event.EventId);
            throw;
        }
    }
}



>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
