namespace FlightWatch.Application.DTOs.FlightMonitoring;

public class FlightUpdateDto
{
    public Guid SubscriptionId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
    public int FlightCount { get; set; }
    public List<FlightDto> Flights { get; set; } = [];
}

