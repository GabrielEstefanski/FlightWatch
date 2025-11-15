<<<<<<< HEAD
namespace FlightWatch.Application.DTOs.FlightMonitoring;

public class FlightUpdateDto
{
    public Guid SubscriptionId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
    public int FlightCount { get; set; }
    public List<FlightDto> Flights { get; set; } = new();
}

=======
namespace FlightWatch.Application.DTOs.FlightMonitoring;

public class FlightUpdateDto
{
    public Guid SubscriptionId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
    public int FlightCount { get; set; }
    public List<FlightDto> Flights { get; set; } = new();
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
