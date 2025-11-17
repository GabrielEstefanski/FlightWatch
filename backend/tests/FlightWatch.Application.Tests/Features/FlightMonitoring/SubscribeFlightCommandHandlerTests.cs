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

public class SubscribeFlightCommandHandlerTests
{
    private readonly Mock<IFlightSubscriptionRepository> _subscriptionRepositoryMock;
    private readonly Mock<IFlightMonitoringService> _monitoringServiceMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly Mock<IEventStore> _eventStoreMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<SubscribeFlightCommandHandler>> _loggerMock;
    private readonly SubscribeFlightCommandHandler _handler;

    public SubscribeFlightCommandHandlerTests()
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
    public async Task Handle_Should_Create_New_Subscription_When_No_Existing_Subscription()
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

        var createdSubscription = new FlightSubscription
        {
            Id = Guid.NewGuid(),
            ConnectionId = command.ConnectionId,
            AreaName = command.AreaName!,
            MinLatitude = command.MinLatitude,
            MaxLatitude = command.MaxLatitude,
            MinLongitude = command.MinLongitude,
            MaxLongitude = command.MaxLongitude,
            UpdateIntervalSeconds = command.UpdateIntervalSeconds,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        };

        var subscriptionDto = new SubscriptionDto
        {
            Id = createdSubscription.Id,
            AreaName = createdSubscription.AreaName,
            MinLatitude = createdSubscription.MinLatitude,
            MaxLatitude = createdSubscription.MaxLatitude,
            MinLongitude = createdSubscription.MinLongitude,
            MaxLongitude = createdSubscription.MaxLongitude,
            IsActive = createdSubscription.IsActive,
            UpdateIntervalSeconds = createdSubscription.UpdateIntervalSeconds
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
            .Returns(subscriptionDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(createdSubscription.Id);
        result.Value.AreaName.Should().Be(command.AreaName);

        _subscriptionRepositoryMock.Verify(
            x => x.GetByConnectionIdAsync(command.ConnectionId),
            Times.Once);

        _subscriptionRepositoryMock.Verify(
            x => x.CreateAsync(It.Is<FlightSubscription>(s =>
                s.ConnectionId == command.ConnectionId &&
                s.AreaName == command.AreaName &&
                s.MinLatitude == command.MinLatitude &&
                s.MaxLatitude == command.MaxLatitude &&
                s.MinLongitude == command.MinLongitude &&
                s.MaxLongitude == command.MaxLongitude &&
                s.UpdateIntervalSeconds == command.UpdateIntervalSeconds &&
                s.IsActive == true)),
            Times.Once);

        _subscriptionRepositoryMock.Verify(
            x => x.DeleteAsync(It.IsAny<Guid>()),
            Times.Never);

        _eventStoreMock.Verify(
            x => x.SaveEventAsync(
                It.Is<FlightSubscriptionCreatedEvent>(e =>
                    e.SubscriptionId == createdSubscription.Id &&
                    e.ConnectionId == command.ConnectionId &&
                    e.AreaName == command.AreaName),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _eventBusMock.Verify(
            x => x.PublishAsync(
                It.Is<FlightSubscriptionCreatedIntegrationEvent>(e =>
                    e.SubscriptionId == createdSubscription.Id &&
                    e.ConnectionId == command.ConnectionId &&
                    e.AreaName == command.AreaName),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Replace_Existing_Subscription_When_One_Exists()
    {
        // Arrange
        var command = new SubscribeFlightCommand(
            ConnectionId: "connection-123",
            AreaName: "New Area",
            MinLatitude: -10.0,
            MaxLatitude: -9.0,
            MinLongitude: -40.0,
            MaxLongitude: -39.0,
            UpdateIntervalSeconds: 60,
            UserId: null);

        var existingSubscription = new FlightSubscription
        {
            Id = Guid.NewGuid(),
            ConnectionId = command.ConnectionId,
            AreaName = "Old Area",
            IsActive = true
        };

        var newSubscription = new FlightSubscription
        {
            Id = Guid.NewGuid(),
            ConnectionId = command.ConnectionId,
            AreaName = command.AreaName!,
            MinLatitude = command.MinLatitude,
            MaxLatitude = command.MaxLatitude,
            MinLongitude = command.MinLongitude,
            MaxLongitude = command.MaxLongitude,
            UpdateIntervalSeconds = command.UpdateIntervalSeconds,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        };

        var subscriptionDto = new SubscriptionDto
        {
            Id = newSubscription.Id,
            AreaName = newSubscription.AreaName
        };

        _subscriptionRepositoryMock
            .Setup(x => x.GetByConnectionIdAsync(command.ConnectionId))
            .ReturnsAsync(existingSubscription);

        _subscriptionRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<FlightSubscription>()))
            .ReturnsAsync(newSubscription);

        _monitoringServiceMock
            .Setup(x => x.UpdateFlightsForSubscriptionAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success());

        _mapperMock
            .Setup(x => x.Map<SubscriptionDto>(It.IsAny<FlightSubscription>()))
            .Returns(subscriptionDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _subscriptionRepositoryMock.Verify(
            x => x.DeleteAsync(existingSubscription.Id),
            Times.Once);

        _subscriptionRepositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<FlightSubscription>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Use_Default_AreaName_When_AreaName_Is_Null()
    {
        // Arrange
        var command = new SubscribeFlightCommand(
            ConnectionId: "connection-123",
            AreaName: null,
            MinLatitude: -10.0,
            MaxLatitude: -9.0,
            MinLongitude: -40.0,
            MaxLongitude: -39.0,
            UpdateIntervalSeconds: 60,
            UserId: null);

        var createdSubscription = new FlightSubscription
        {
            Id = Guid.NewGuid(),
            ConnectionId = command.ConnectionId,
            AreaName = "Custom Area",
            IsActive = true
        };

        var subscriptionDto = new SubscriptionDto
        {
            Id = createdSubscription.Id,
            AreaName = "Custom Area"
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
            .Returns(subscriptionDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _subscriptionRepositoryMock.Verify(
            x => x.CreateAsync(It.Is<FlightSubscription>(s =>
                s.AreaName == "Custom Area")),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Fetch_Initial_Flight_Data_After_Creating_Subscription()
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

        var createdSubscription = new FlightSubscription
        {
            Id = Guid.NewGuid(),
            ConnectionId = command.ConnectionId,
            AreaName = command.AreaName!,
            IsActive = true
        };

        var subscriptionDto = new SubscriptionDto
        {
            Id = createdSubscription.Id,
            AreaName = createdSubscription.AreaName
        };

        _subscriptionRepositoryMock
            .Setup(x => x.GetByConnectionIdAsync(command.ConnectionId))
            .ReturnsAsync((FlightSubscription?)null);

        _subscriptionRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<FlightSubscription>()))
            .ReturnsAsync(createdSubscription);

        _monitoringServiceMock
            .Setup(x => x.UpdateFlightsForSubscriptionAsync(createdSubscription.Id))
            .ReturnsAsync(Result.Success());

        _mapperMock
            .Setup(x => x.Map<SubscriptionDto>(It.IsAny<FlightSubscription>()))
            .Returns(subscriptionDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        await Task.Delay(100);

        _monitoringServiceMock.Verify(
            x => x.UpdateFlightsForSubscriptionAsync(createdSubscription.Id),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Handle_Flight_Data_Fetch_Failure_Gracefully()
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

        var createdSubscription = new FlightSubscription
        {
            Id = Guid.NewGuid(),
            ConnectionId = command.ConnectionId,
            AreaName = command.AreaName!,
            IsActive = true
        };

        var subscriptionDto = new SubscriptionDto
        {
            Id = createdSubscription.Id,
            AreaName = createdSubscription.AreaName
        };

        _subscriptionRepositoryMock
            .Setup(x => x.GetByConnectionIdAsync(command.ConnectionId))
            .ReturnsAsync((FlightSubscription?)null);

        _subscriptionRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<FlightSubscription>()))
            .ReturnsAsync(createdSubscription);

        _monitoringServiceMock
            .Setup(x => x.UpdateFlightsForSubscriptionAsync(createdSubscription.Id))
            .ReturnsAsync(Result.Failure(Error.Failure("FlightFetchError", "Failed to fetch flights")));

        _mapperMock
            .Setup(x => x.Map<SubscriptionDto>(It.IsAny<FlightSubscription>()))
            .Returns(subscriptionDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        await Task.Delay(100);

        _monitoringServiceMock.Verify(
            x => x.UpdateFlightsForSubscriptionAsync(createdSubscription.Id),
            Times.Once);
    }
}

