using FlightWatch.Application.DTOs;
using FlightWatch.Application.Events;
using FlightWatch.Application.Interfaces;
using FlightWatch.Infrastructure.EventHandlers;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FlightWatch.Infrastructure.Tests.EventHandlers;

public class FlightDataUpdatedEventHandlerTests
{
    private readonly Mock<ILogger<FlightDataUpdatedEventHandler>> _loggerMock;
    private readonly Mock<IFlightNotificationService> _notificationServiceMock;
    private readonly FlightDataUpdatedEventHandler _handler;

    public FlightDataUpdatedEventHandlerTests()
    {
        _loggerMock = new Mock<ILogger<FlightDataUpdatedEventHandler>>();
        _notificationServiceMock = new Mock<IFlightNotificationService>();

        _handler = new FlightDataUpdatedEventHandler(
            _loggerMock.Object,
            _notificationServiceMock.Object);
    }

    [Fact]
    public async Task Consume_Should_Notify_Client_With_Flight_Data_When_Event_Is_Received()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var connectionId = "connection-123";

        var integrationEvent = new FlightDataUpdatedIntegrationEvent
        {
            SubscriptionId = subscriptionId,
            ConnectionId = connectionId,
            FlightCount = 2,
            AreaName = "Test Area",
            Flights =
            [
                new FlightData
                {
                    FlightNumber = "AA123",
                    Airline = "US",
                    Latitude = -10.0,
                    Longitude = -40.0,
                    Altitude = 10000.0,
                    Velocity = 500.0,
                    Direction = 90.0,
                    FlightStatus = "en-route",
                    IsLive = true,
                    Category = 3
                },
                new FlightData
                {
                    FlightNumber = "BB456",
                    Airline = "BR",
                    Latitude = -9.5,
                    Longitude = -39.5,
                    Altitude = 12000.0,
                    Velocity = 600.0,
                    Direction = 180.0,
                    FlightStatus = "en-route",
                    IsLive = true,
                    Category = 4
                }
            ]
        };

        var consumeContextMock = new Mock<ConsumeContext<FlightDataUpdatedIntegrationEvent>>();
        consumeContextMock.Setup(x => x.Message).Returns(integrationEvent);

        _notificationServiceMock
            .Setup(x => x.NotifyFlightUpdatesAsync(
                connectionId,
                subscriptionId,
                It.IsAny<List<FlightDto>>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Consume(consumeContextMock.Object);

        // Assert
        _notificationServiceMock.Verify(
            x => x.NotifyFlightUpdatesAsync(
                connectionId,
                subscriptionId,
                It.Is<List<FlightDto>>(flights =>
                    flights.Count == 2 &&
                    flights[0].FlightNumber == "AA123" &&
                    flights[0].Airline == "US" &&
                    flights[0].Latitude == -10.0 &&
                    flights[0].Longitude == -40.0 &&
                    flights[0].Altitude == 10000.0 &&
                    flights[0].Velocity == 500.0 &&
                    flights[0].Direction == 90.0 &&
                    flights[0].FlightStatus == "en-route" &&
                    flights[0].IsLive == true &&
                    flights[0].Category == 3 &&
                    flights[1].FlightNumber == "BB456" &&
                    flights[1].Airline == "BR" &&
                    flights[1].Category == 4)),
            Times.Once);
    }

    [Fact]
    public async Task Consume_Should_Map_Category_To_CategoryDescription()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var connectionId = "connection-123";

        var integrationEvent = new FlightDataUpdatedIntegrationEvent
        {
            SubscriptionId = subscriptionId,
            ConnectionId = connectionId,
            FlightCount = 1,
            AreaName = "Test Area",
            Flights =
            [
                new FlightData
                {
                    FlightNumber = "AA123",
                    Category = 3
                }
            ]
        };

        var consumeContextMock = new Mock<ConsumeContext<FlightDataUpdatedIntegrationEvent>>();
        consumeContextMock.Setup(x => x.Message).Returns(integrationEvent);

        _notificationServiceMock
            .Setup(x => x.NotifyFlightUpdatesAsync(
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<List<FlightDto>>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Consume(consumeContextMock.Object);

        // Assert
        _notificationServiceMock.Verify(
            x => x.NotifyFlightUpdatesAsync(
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.Is<List<FlightDto>>(flights =>
                    flights[0].Category == 3 &&
                    flights[0].CategoryDescription == "Small")),
            Times.Once);
    }

    [Fact]
    public async Task Consume_Should_Handle_Empty_Flights_List()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var connectionId = "connection-123";

        var integrationEvent = new FlightDataUpdatedIntegrationEvent
        {
            SubscriptionId = subscriptionId,
            ConnectionId = connectionId,
            FlightCount = 0,
            AreaName = "Test Area",
            Flights = []
        };

        var consumeContextMock = new Mock<ConsumeContext<FlightDataUpdatedIntegrationEvent>>();
        consumeContextMock.Setup(x => x.Message).Returns(integrationEvent);

        _notificationServiceMock
            .Setup(x => x.NotifyFlightUpdatesAsync(
                connectionId,
                subscriptionId,
                It.IsAny<List<FlightDto>>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Consume(consumeContextMock.Object);

        // Assert
        _notificationServiceMock.Verify(
            x => x.NotifyFlightUpdatesAsync(
                connectionId,
                subscriptionId,
                It.Is<List<FlightDto>>(flights => flights.Count == 0)),
            Times.Once);
    }

    [Fact]
    public async Task Consume_Should_Log_Information_When_Processing_Event()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var connectionId = "connection-123";

        var integrationEvent = new FlightDataUpdatedIntegrationEvent
        {
            SubscriptionId = subscriptionId,
            ConnectionId = connectionId,
            FlightCount = 1,
            AreaName = "Test Area",
            Flights =
            [
                new FlightData
                {
                    FlightNumber = "AA123"
                }
            ]
        };

        var consumeContextMock = new Mock<ConsumeContext<FlightDataUpdatedIntegrationEvent>>();
        consumeContextMock.Setup(x => x.Message).Returns(integrationEvent);

        _notificationServiceMock
            .Setup(x => x.NotifyFlightUpdatesAsync(
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<List<FlightDto>>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Consume(consumeContextMock.Object);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processing FlightDataUpdatedEvent")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("successfully sent")),
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

        var integrationEvent = new FlightDataUpdatedIntegrationEvent
        {
            SubscriptionId = subscriptionId,
            ConnectionId = connectionId,
            FlightCount = 1,
            AreaName = "Test Area",
            Flights =
            [
                new FlightData
                {
                    FlightNumber = "AA123"
                }
            ]
        };

        var consumeContextMock = new Mock<ConsumeContext<FlightDataUpdatedIntegrationEvent>>();
        consumeContextMock.Setup(x => x.Message).Returns(integrationEvent);

        var expectedException = new Exception("Notification failed");

        _notificationServiceMock
            .Setup(x => x.NotifyFlightUpdatesAsync(
                connectionId,
                subscriptionId,
                It.IsAny<List<FlightDto>>()))
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

