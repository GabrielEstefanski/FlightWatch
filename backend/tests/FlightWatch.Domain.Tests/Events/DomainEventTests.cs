using FlightWatch.Domain.Events;
using Xunit;

namespace FlightWatch.Domain.Tests.Events;

public class DomainEventTests
{
    [Fact]
    public void DomainEvent_Should_Have_EventId()
    {
        // Arrange & Act
        var domainEvent = new FlightSubscriptionCreatedEvent
        {
            SubscriptionId = Guid.NewGuid(),
            ConnectionId = "test-connection",
            AreaName = "Test Area"
        };

        // Assert
        Assert.NotEqual(Guid.Empty, domainEvent.EventId);
    }

    [Fact]
    public void DomainEvent_Should_Have_OccurredOn()
    {
        // Arrange & Act
        var domainEvent = new FlightSubscriptionCreatedEvent
        {
            SubscriptionId = Guid.NewGuid(),
            ConnectionId = "test-connection",
            AreaName = "Test Area"
        };

        // Assert
        Assert.True(domainEvent.OccurredOn <= DateTime.UtcNow);
    }
}

