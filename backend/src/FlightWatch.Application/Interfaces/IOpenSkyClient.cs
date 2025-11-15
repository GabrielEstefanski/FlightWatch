using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs;

namespace FlightWatch.Application.Interfaces;

public interface IOpenSkyClient
{
    Task<Result<IEnumerable<FlightDto>>> GetAllFlightsAsync();
    Task<Result<IEnumerable<FlightDto>>> GetFlightsByBoundingBoxAsync(
        double minLatitude,
        double maxLatitude,
        double minLongitude,
        double maxLongitude);
}

