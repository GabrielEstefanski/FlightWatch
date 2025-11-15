using AutoMapper;
using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs.FlightMonitoring;
using FlightWatch.Application.Events;
using FlightWatch.Application.Interfaces;
using FlightWatch.Domain.Entities;
using FlightWatch.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Application.Features.FlightMonitoring.Commands.Subscribe;

public class SubscribeFlightCommandHandler(
    IFlightSubscriptionRepository subscriptionRepository,
    IFlightMonitoringService monitoringService,
    IEventBus eventBus,
    IEventStore eventStore,
    IMapper mapper,
    ILogger<SubscribeFlightCommandHandler> logger) : IRequestHandler<SubscribeFlightCommand, Result<SubscriptionDto>>
{
    private readonly IFlightSubscriptionRepository _subscriptionRepository = subscriptionRepository;
    private readonly IFlightMonitoringService _monitoringService = monitoringService;
    private readonly IEventBus _eventBus = eventBus;
    private readonly IEventStore _eventStore = eventStore;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<SubscribeFlightCommandHandler> _logger = logger;

    public async Task<Result<SubscriptionDto>> Handle(SubscribeFlightCommand request, CancellationToken cancellationToken)
    {
        var existingSubscription = await _subscriptionRepository.GetByConnectionIdAsync(request.ConnectionId);

        if (existingSubscription != null)
        {
            await _subscriptionRepository.DeleteAsync(existingSubscription.Id);
        }

        var subscription = new FlightSubscription
        {
            ConnectionId = request.ConnectionId,
            UserId = request.UserId,
            AreaName = request.AreaName ?? "Custom Area",
            MinLatitude = request.MinLatitude,
            MaxLatitude = request.MaxLatitude,
            MinLongitude = request.MinLongitude,
            MaxLongitude = request.MaxLongitude,
            UpdateIntervalSeconds = request.UpdateIntervalSeconds,
            IsActive = true
        };

        subscription = await _subscriptionRepository.CreateAsync(subscription);

        _logger.LogInformation(
            "Flight subscription created. SubscriptionId: {SubscriptionId}, Area: {AreaName}",
            subscription.Id,
            subscription.AreaName);

        var domainEvent = new FlightSubscriptionCreatedEvent
        {
            SubscriptionId = subscription.Id,
            ConnectionId = subscription.ConnectionId,
            UserId = subscription.UserId,
            AreaName = subscription.AreaName,
            MinLatitude = subscription.MinLatitude,
            MaxLatitude = subscription.MaxLatitude,
            MinLongitude = subscription.MinLongitude,
            MaxLongitude = subscription.MaxLongitude,
            UpdateIntervalSeconds = subscription.UpdateIntervalSeconds
        };

        await _eventStore.SaveEventAsync(domainEvent, cancellationToken);

        var integrationEvent = new FlightSubscriptionCreatedIntegrationEvent
        {
            SubscriptionId = subscription.Id,
            ConnectionId = subscription.ConnectionId,
            UserId = subscription.UserId,
            AreaName = subscription.AreaName,
            MinLatitude = subscription.MinLatitude,
            MaxLatitude = subscription.MaxLatitude,
            MinLongitude = subscription.MinLongitude,
            MaxLongitude = subscription.MaxLongitude
        };

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        _logger.LogInformation(
            "Flight subscription event published. SubscriptionId: {SubscriptionId}, Area: {AreaName}",
            subscription.Id,
            subscription.AreaName);

        _logger.LogInformation(
            "Fetching initial flight data for subscription {SubscriptionId}",
            subscription.Id);

        // Buscar voos imediatamente em background (sem delay)
        // Fire-and-forget para não bloquear a resposta ao cliente
        _ = Task.Run(async () =>
        {
            try
            {
                // Sem delay - busca imediata!
                var updateResult = await _monitoringService.UpdateFlightsForSubscriptionAsync(subscription.Id);
                
                if (updateResult.IsFailure)
                {
                    _logger.LogWarning(
                        "Failed to fetch initial flight data for subscription {SubscriptionId}: {Error}",
                        subscription.Id,
                        updateResult.Error.Message);
                }
                else
                {
                    _logger.LogInformation(
                        "Initial flight data fetched successfully for subscription {SubscriptionId}",
                        subscription.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception while fetching initial flight data for subscription {SubscriptionId}",
                    subscription.Id);
            }
        }, cancellationToken);

        return Result.Success(_mapper.Map<SubscriptionDto>(subscription));
    }
}

