namespace FlightWatch.Domain.Entities;

public class FlightUpdate
{
    public Guid SubscriptionId { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
    public int FlightCount { get; set; }
    public List<Flight> Flights { get; set; } = [];
}

