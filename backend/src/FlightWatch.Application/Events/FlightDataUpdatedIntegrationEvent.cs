namespace FlightWatch.Application.Events;

public class FlightDataUpdatedIntegrationEvent : IntegrationEvent
{
    public Guid SubscriptionId { get; init; }
    public string ConnectionId { get; init; } = string.Empty;
    public int FlightCount { get; init; }
    public string AreaName { get; init; } = string.Empty;
    public List<FlightData> Flights { get; init; } = [];
}

public class FlightData
{
    public string FlightNumber { get; init; } = string.Empty;
    public string Airline { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string Origin { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public double? Altitude { get; init; }
    public double? Velocity { get; init; }
    public double? Direction { get; init; }
    public string? FlightStatus { get; init; }
    public bool IsLive { get; init; }
    public int? Category { get; init; }
}

