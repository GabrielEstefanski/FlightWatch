using AutoMapper;
using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs.FlightMonitoring;
using FlightWatch.Application.Events;
using FlightWatch.Application.Features.FlightMonitoring.Commands.Subscribe;
using FlightWatch.Application.Interfaces;
using FlightWatch.Domain.Entities;
using FlightWatch.Domain.Events;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FlightWatch.Application.Tests.Features.FlightMonitoring;

/// <summary>
/// Testes de integração que verificam o fluxo completo do SubscribeFlightCommandHandler,
/// incluindo EventStore e EventBus
/// </summary>
public class SubscribeFlightCommandHandlerIntegrationTests
{
    private readonly Mock<IFlightSubscriptionRepository> _subscriptionRepositoryMock;
    private readonly Mock<IFlightMonitoringService> _monitoringServiceMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly Mock<IEventStore> _eventStoreMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<SubscribeFlightCommandHandler>> _loggerMock;
    private readonly SubscribeFlightCommandHandler _handler;

    public SubscribeFlightCommandHandlerIntegrationTests()
    {
        _subscriptionRepositoryMock = new Mock<IFlightSubscriptionRepository>();
        _monitoringServiceMock = new Mock<IFlightMonitoringService>();
        _eventBusMock = new Mock<IEventBus>();
        _eventStoreMock = new Mock<IEventStore>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<SubscribeFlightCommandHandler>>();

        _handler = new SubscribeFlightCommandHandler(
            _subscriptionRepositoryMock.Object,
            _monitoringServiceMock.Object,
            _eventBusMock.Object,
            _eventStoreMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Save_Domain_Event_To_EventStore()
    {
        // Arrange
        var command = new SubscribeFlightCommand(
            ConnectionId: "connection-123",
            AreaName: "Test Area",
            MinLatitude: -10.0,
            MaxLatitude: -9.0,
            MinLongitude: -40.0,
            MaxLongitude: -39.0,
            UpdateIntervalSeconds: 60,
            UserId: null);

        var subscriptionId = Guid.NewGuid();
        var createdSubscription = new FlightSubscription
        {
            Id = subscriptionId,
            ConnectionId = command.ConnectionId,
            AreaName = command.AreaName!,
            MinLatitude = command.MinLatitude,
            MaxLatitude = command.MaxLatitude,
            MinLongitude = command.MinLongitude,
            MaxLongitude = command.MaxLongitude,
            UpdateIntervalSeconds = command.UpdateIntervalSeconds,
            IsActive = true
        };

        _subscriptionRepositoryMock
            .Setup(x => x.GetByConnectionIdAsync(command.ConnectionId))
            .ReturnsAsync((FlightSubscription?)null);

        _subscriptionRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<FlightSubscription>()))
            .ReturnsAsync(createdSubscription);

        _monitoringServiceMock
            .Setup(x => x.UpdateFlightsForSubscriptionAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success());

        _mapperMock
            .Setup(x => x.Map<SubscriptionDto>(It.IsAny<FlightSubscription>()))
            .Returns(new SubscriptionDto { Id = subscriptionId });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert - Verificar que o evento de domínio foi salvo no EventStore
        _eventStoreMock.Verify(
            x => x.SaveEventAsync(
                It.Is<FlightSubscriptionCreatedEvent>(e =>
                    e.SubscriptionId == subscriptionId &&
                    e.ConnectionId == command.ConnectionId &&
                    e.AreaName == command.AreaName &&
                    e.MinLatitude == command.MinLatitude &&
                    e.MaxLatitude == command.MaxLatitude &&
                    e.MinLongitude == command.MinLongitude &&
                    e.MaxLongitude == command.MaxLongitude &&
                    e.UpdateIntervalSeconds == command.UpdateIntervalSeconds),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Publish_Integration_Event_To_EventBus()
    {
        // Arrange
        var command = new SubscribeFlightCommand(
            ConnectionId: "connection-123",
            AreaName: "Test Area",
            MinLatitude: -10.0,
            MaxLatitude: -9.0,
            MinLongitude: -40.0,
            MaxLongitude: -39.0,
            UpdateIntervalSeconds: 60,
            UserId: Guid.NewGuid());

        var subscriptionId = Guid.NewGuid();
        var createdSubscription = new FlightSubscription
        {
            Id = subscriptionId,
            ConnectionId = command.ConnectionId,
            UserId = command.UserId,
            AreaName = command.AreaName!,
            MinLatitude = command.MinLatitude,
            MaxLatitude = command.MaxLatitude,
            MinLongitude = command.MinLongitude,
            MaxLongitude = command.MaxLongitude,
            UpdateIntervalSeconds = command.UpdateIntervalSeconds,
            IsActive = true
        };

        _subscriptionRepositoryMock
            .Setup(x => x.GetByConnectionIdAsync(command.ConnectionId))
            .ReturnsAsync((FlightSubscription?)null);

        _subscriptionRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<FlightSubscription>()))
            .ReturnsAsync(createdSubscription);

        _monitoringServiceMock
            .Setup(x => x.UpdateFlightsForSubscriptionAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success());

        _mapperMock
            .Setup(x => x.Map<SubscriptionDto>(It.IsAny<FlightSubscription>()))
            .Returns(new SubscriptionDto { Id = subscriptionId });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert - Verificar que o evento de integração foi publicado no EventBus
        _eventBusMock.Verify(
            x => x.PublishAsync(
                It.Is<FlightSubscriptionCreatedIntegrationEvent>(e =>
                    e.SubscriptionId == subscriptionId &&
                    e.ConnectionId == command.ConnectionId &&
                    e.UserId == command.UserId &&
                    e.AreaName == command.AreaName &&
                    e.MinLatitude == command.MinLatitude &&
                    e.MaxLatitude == command.MaxLatitude &&
                    e.MinLongitude == command.MinLongitude &&
                    e.MaxLongitude == command.MaxLongitude),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Execute_Complete_Flow_In_Correct_Order()
    {
        // Arrange
        var command = new SubscribeFlightCommand(
            ConnectionId: "connection-123",
            AreaName: "Test Area",
            MinLatitude: -10.0,
            MaxLatitude: -9.0,
            MinLongitude: -40.0,
            MaxLongitude: -39.0,
            UpdateIntervalSeconds: 60,
            UserId: null);

        var subscriptionId = Guid.NewGuid();
        var createdSubscription = new FlightSubscription
        {
            Id = subscriptionId,
            ConnectionId = command.ConnectionId,
            AreaName = command.AreaName!,
            IsActive = true
        };

        var executionOrder = new List<string>();

        _subscriptionRepositoryMock
            .Setup(x => x.GetByConnectionIdAsync(command.ConnectionId))
            .ReturnsAsync((FlightSubscription?)null)
            .Callback(() => executionOrder.Add("1. GetByConnectionIdAsync"));

        _subscriptionRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<FlightSubscription>()))
            .ReturnsAsync(createdSubscription)
            .Callback(() => executionOrder.Add("2. CreateAsync"));

        _eventStoreMock
            .Setup(x => x.SaveEventAsync(It.IsAny<FlightSubscriptionCreatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(() => executionOrder.Add("3. SaveEventAsync (Domain Event)"));

        _eventBusMock
            .Setup(x => x.PublishAsync(It.IsAny<FlightSubscriptionCreatedIntegrationEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(() => executionOrder.Add("4. PublishAsync (Integration Event)"));

        _monitoringServiceMock
            .Setup(x => x.UpdateFlightsForSubscriptionAsync(subscriptionId))
            .ReturnsAsync(Result.Success())
            .Callback(() => executionOrder.Add("5. UpdateFlightsForSubscriptionAsync"));

        _mapperMock
            .Setup(x => x.Map<SubscriptionDto>(It.IsAny<FlightSubscription>()))
            .Returns(new SubscriptionDto { Id = subscriptionId });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Aguardar um pouco para o Task.Run completar
        await Task.Delay(150);

        // Verificar ordem de execução
        executionOrder.Should().ContainInOrder(
            "1. GetByConnectionIdAsync",
            "2. CreateAsync",
            "3. SaveEventAsync (Domain Event)",
            "4. PublishAsync (Integration Event)");

        // UpdateFlightsForSubscriptionAsync deve ser executado (pode estar em qualquer posição após CreateAsync)
        executionOrder.Should().Contain("5. UpdateFlightsForSubscriptionAsync");
    }

    [Fact]
    public async Task Handle_Should_Handle_EventStore_Failure_Gracefully()
    {
        // Arrange
        var command = new SubscribeFlightCommand(
            ConnectionId: "connection-123",
            AreaName: "Test Area",
            MinLatitude: -10.0,
            MaxLatitude: -9.0,
            MinLongitude: -40.0,
            MaxLongitude: -39.0,
            UpdateIntervalSeconds: 60,
            UserId: null);

        var subscriptionId = Guid.NewGuid();
        var createdSubscription = new FlightSubscription
        {
            Id = subscriptionId,
            ConnectionId = command.ConnectionId,
            AreaName = command.AreaName!,
            IsActive = true
        };

        _subscriptionRepositoryMock
            .Setup(x => x.GetByConnectionIdAsync(command.ConnectionId))
            .ReturnsAsync((FlightSubscription?)null);

        _subscriptionRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<FlightSubscription>()))
            .ReturnsAsync(createdSubscription);

        _eventStoreMock
            .Setup(x => x.SaveEventAsync(It.IsAny<FlightSubscriptionCreatedEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("EventStore failure"));

        _mapperMock
            .Setup(x => x.Map<SubscriptionDto>(It.IsAny<FlightSubscription>()))
            .Returns(new SubscriptionDto { Id = subscriptionId });

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

        // Subscription ainda deve ser criada antes da falha
        _subscriptionRepositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<FlightSubscription>()),
            Times.Once);
    }
}

