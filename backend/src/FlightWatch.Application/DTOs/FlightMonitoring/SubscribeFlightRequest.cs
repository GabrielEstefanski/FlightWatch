using System.ComponentModel.DataAnnotations;

namespace FlightWatch.Application.DTOs.FlightMonitoring;

public class SubscribeFlightRequest
{
    public string? AreaName { get; set; }

    [Range(-90, 90)]
    public double MinLatitude { get; set; }

    [Range(-90, 90)]
    public double MaxLatitude { get; set; }

    [Range(-180, 180)]
    public double MinLongitude { get; set; }

    [Range(-180, 180)]
    public double MaxLongitude { get; set; }

    [Range(30, 600)]
    public int UpdateIntervalSeconds { get; set; } = 60;
}

