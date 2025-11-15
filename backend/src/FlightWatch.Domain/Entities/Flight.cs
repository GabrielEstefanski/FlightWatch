namespace FlightWatch.Domain.Entities;

public class Flight
{
    public string FlightNumber { get; set; } = string.Empty;
    public string Airline { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public double? Direction { get; set; }
    public double? Altitude { get; set; }
    public double? Velocity { get; set; }
    public string? FlightStatus { get; set; }
    public bool IsLive { get; set; }
    public int? Category { get; set; }
}

