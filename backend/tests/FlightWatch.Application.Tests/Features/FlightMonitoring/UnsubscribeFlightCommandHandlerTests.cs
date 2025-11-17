using FlightWatch.Application.Common;
using FlightWatch.Application.Features.FlightMonitoring.Commands.Unsubscribe;
using FlightWatch.Application.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FlightWatch.Application.Tests.Features.FlightMonitoring;

public class UnsubscribeFlightCommandHandlerTests
{
    private readonly Mock<IFlightSubscriptionRepository> _subscriptionRepositoryMock;
    private readonly Mock<ILogger<UnsubscribeFlightCommandHandler>> _loggerMock;
    private readonly UnsubscribeFlightCommandHandler _handler;

    public UnsubscribeFlightCommandHandlerTests()
    {
        _subscriptionRepositoryMock = new Mock<IFlightSubscriptionRepository>();
        _loggerMock = new Mock<ILogger<UnsubscribeFlightCommandHandler>>();

        _handler = new UnsubscribeFlightCommandHandler(
            _subscriptionRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Delete_Subscription_By_ConnectionId()
    {
        // Arrange
        var command = new UnsubscribeFlightCommand("connection-123");

        _subscriptionRepositoryMock
            .Setup(x => x.DeleteByConnectionIdAsync(command.ConnectionId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _subscriptionRepositoryMock.Verify(
            x => x.DeleteByConnectionIdAsync(command.ConnectionId),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_Even_If_Subscription_Does_Not_Exist()
    {
        // Arrange
        var command = new UnsubscribeFlightCommand("non-existent-connection");

        _subscriptionRepositoryMock
            .Setup(x => x.DeleteByConnectionIdAsync(command.ConnectionId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _subscriptionRepositoryMock.Verify(
            x => x.DeleteByConnectionIdAsync(command.ConnectionId),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Subscription_Removal()
    {
        // Arrange
        var command = new UnsubscribeFlightCommand("connection-123");

        _subscriptionRepositoryMock
            .Setup(x => x.DeleteByConnectionIdAsync(command.ConnectionId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Flight subscription removed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}

