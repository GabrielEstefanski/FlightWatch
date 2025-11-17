using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs;
using FlightWatch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Application.Features.Flights.Queries.GetLiveFlightsByCountry;

public class GetLiveFlightsByCountryQueryHandler : IRequestHandler<GetLiveFlightsByCountryQuery, Result<IEnumerable<FlightDto>>>
{
    private readonly IOpenSkyClient _openSkyClient;
    private readonly ILogger<GetLiveFlightsByCountryQueryHandler> _logger;

    public GetLiveFlightsByCountryQueryHandler(
        IOpenSkyClient openSkyClient,
        ILogger<GetLiveFlightsByCountryQueryHandler> logger)
    {
        _openSkyClient = openSkyClient;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<FlightDto>>> Handle(GetLiveFlightsByCountryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching live flights from country: {Country}", request.Country);

        var result = await _openSkyClient.GetAllFlightsAsync();

        if (result.IsFailure)
        {
            return result;
        }

        var filteredFlights = result.Value
            .Where(f => f.Airline?.Equals(request.Country, StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

        _logger.LogInformation(
            "Found {Count} flights from {Country}",
            filteredFlights.Count,
            request.Country);

        return Result.Success<IEnumerable<FlightDto>>(filteredFlights);
    }
}

