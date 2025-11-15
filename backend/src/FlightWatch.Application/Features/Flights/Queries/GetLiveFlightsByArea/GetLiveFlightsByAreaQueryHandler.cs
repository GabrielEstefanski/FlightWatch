using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs;
using FlightWatch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Application.Features.Flights.Queries.GetLiveFlightsByArea;

public class GetLiveFlightsByAreaQueryHandler(
    IOpenSkyClient openSkyClient,
    ILogger<GetLiveFlightsByAreaQueryHandler> logger) : IRequestHandler<GetLiveFlightsByAreaQuery, Result<IEnumerable<FlightDto>>>
{
    private readonly IOpenSkyClient _openSkyClient = openSkyClient;
    private readonly ILogger<GetLiveFlightsByAreaQueryHandler> _logger = logger;

    public async Task<Result<IEnumerable<FlightDto>>> Handle(GetLiveFlightsByAreaQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Fetching live flights in area: ({MinLat},{MinLon}) to ({MaxLat},{MaxLon})",
            request.MinLatitude,
            request.MinLongitude,
            request.MaxLatitude,
            request.MaxLongitude);

        return await _openSkyClient.GetFlightsByBoundingBoxAsync(
            request.MinLatitude,
            request.MaxLatitude,
            request.MinLongitude,
            request.MaxLongitude);
    }
}

