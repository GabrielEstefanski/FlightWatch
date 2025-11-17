using FlightWatch.Application.Events;
using FlightWatch.Application.Interfaces;
using FlightWatch.Infrastructure.EventHandlers;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FlightWatch.Infrastructure.Tests.EventHandlers;

public class FlightSubscriptionCreatedEventHandlerTests
{
    private readonly Mock<ILogger<FlightSubscriptionCreatedEventHandler>> _loggerMock;
    private readonly Mock<IFlightNotificationService> _notificationServiceMock;
    private readonly FlightSubscriptionCreatedEventHandler _handler;

    public FlightSubscriptionCreatedEventHandlerTests()
    {
        _loggerMock = new Mock<ILogger<FlightSubscriptionCreatedEventHandler>>();
        _notificationServiceMock = new Mock<IFlightNotificationService>();

        _handler = new FlightSubscriptionCreatedEventHandler(
            _loggerMock.Object,
            _notificationServiceMock.Object);
    }

    [Fact]
    public async Task Consume_Should_Notify_Client_When_Event_Is_Received()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var connectionId = "connection-123";
        var areaName = "Test Area";

        var integrationEvent = new FlightSubscriptionCreatedIntegrationEvent
        {
            SubscriptionId = subscriptionId,
            ConnectionId = connectionId,
            AreaName = areaName,
            MinLatitude = -10.0,
            MaxLatitude = -9.0,
            MinLongitude = -40.0,
            MaxLongitude = -39.0
        };

        var consumeContextMock = new Mock<ConsumeContext<FlightSubscriptionCreatedIntegrationEvent>>();
        consumeContextMock.Setup(x => x.Message).Returns(integrationEvent);

        _notificationServiceMock
            .Setup(x => x.NotifySubscriptionCreatedAsync(
                connectionId,
                subscriptionId,
                areaName))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Consume(consumeContextMock.Object);

        // Assert
        _notificationServiceMock.Verify(
            x => x.NotifySubscriptionCreatedAsync(
                connectionId,
                subscriptionId,
                areaName),
            Times.Once);
    }

    [Fact]
    public async Task Consume_Should_Log_Information_When_Processing_Event()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var connectionId = "connection-123";
        var areaName = "Test Area";

        var integrationEvent = new FlightSubscriptionCreatedIntegrationEvent
        {
            SubscriptionId = subscriptionId,
            ConnectionId = connectionId,
            AreaName = areaName
        };

        var consumeContextMock = new Mock<ConsumeContext<FlightSubscriptionCreatedIntegrationEvent>>();
        consumeContextMock.Setup(x => x.Message).Returns(integrationEvent);

        _notificationServiceMock
            .Setup(x => x.NotifySubscriptionCreatedAsync(
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Consume(consumeContextMock.Object);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processing FlightSubscriptionCreatedEvent")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("successfully processed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_Should_Throw_Exception_When_Notification_Fails()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var connectionId = "connection-123";
        var areaName = "Test Area";

        var integrationEvent = new FlightSubscriptionCreatedIntegrationEvent
        {
            SubscriptionId = subscriptionId,
            ConnectionId = connectionId,
            AreaName = areaName
        };

        var consumeContextMock = new Mock<ConsumeContext<FlightSubscriptionCreatedIntegrationEvent>>();
        consumeContextMock.Setup(x => x.Message).Returns(integrationEvent);

        var expectedException = new Exception("Notification failed");

        _notificationServiceMock
            .Setup(x => x.NotifySubscriptionCreatedAsync(
                connectionId,
                subscriptionId,
                areaName))
            .ThrowsAsync(expectedException);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Consume(consumeContextMock.Object));

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error processing")),
                It.Is<Exception>(e => e == expectedException),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}

