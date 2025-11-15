namespace FlightWatch.Domain.Entities;

public class FlightSubscription
{
    public Guid Id { get; set; }
    public string ConnectionId { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public double MinLatitude { get; set; }
    public double MaxLatitude { get; set; }
    public double MinLongitude { get; set; }
    public double MaxLongitude { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public int UpdateIntervalSeconds { get; set; } = 60;
}

