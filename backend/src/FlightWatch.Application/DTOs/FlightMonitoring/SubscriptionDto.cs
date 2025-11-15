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

