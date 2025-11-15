using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs;
using FlightWatch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Application.Features.Flights.Queries.GetLiveFlights;

public class GetLiveFlightsQueryHandler(
    IOpenSkyClient openSkyClient,
    ILogger<GetLiveFlightsQueryHandler> logger) : IRequestHandler<GetLiveFlightsQuery, Result<IEnumerable<FlightDto>>>
{
    private readonly IOpenSkyClient _openSkyClient = openSkyClient;
    private readonly ILogger<GetLiveFlightsQueryHandler> _logger = logger;

    public async Task<Result<IEnumerable<FlightDto>>> Handle(GetLiveFlightsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all live flights from OpenSky Network");

        return await _openSkyClient.GetAllFlightsAsync();
    }
}

