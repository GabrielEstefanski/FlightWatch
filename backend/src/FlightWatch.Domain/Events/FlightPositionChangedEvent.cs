namespace FlightWatch.Domain.Events;

public class FlightPositionChangedEvent : DomainEvent
{
    public string FlightNumber { get; init; } = string.Empty;
    public double OldLatitude { get; init; }
    public double OldLongitude { get; init; }
    public double NewLatitude { get; init; }
    public double NewLongitude { get; init; }
    public double? Altitude { get; init; }
    public double? Velocity { get; init; }
    public double? Direction { get; init; }
}



