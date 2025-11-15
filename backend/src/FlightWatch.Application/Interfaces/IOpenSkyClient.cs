<<<<<<< HEAD
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

=======
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

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
