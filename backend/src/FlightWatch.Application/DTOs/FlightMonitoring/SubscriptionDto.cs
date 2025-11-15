<<<<<<< HEAD
namespace FlightWatch.Application.DTOs.FlightMonitoring;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public double MinLatitude { get; set; }
    public double MaxLatitude { get; set; }
    public double MinLongitude { get; set; }
    public double MaxLongitude { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public int UpdateIntervalSeconds { get; set; }
}

=======
namespace FlightWatch.Application.DTOs.FlightMonitoring;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public double MinLatitude { get; set; }
    public double MaxLatitude { get; set; }
    public double MinLongitude { get; set; }
    public double MaxLongitude { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public int UpdateIntervalSeconds { get; set; }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
