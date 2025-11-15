using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using FlightWatch.Application.Common;
using FlightWatch.Application.Configuration;
using FlightWatch.Application.DTOs;
using FlightWatch.Application.Interfaces;
using FlightWatch.Infrastructure.ExternalServices.OpenSky.Models;
using FlightWatch.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FlightWatch.Infrastructure.Configuration;

namespace FlightWatch.Infrastructure.ExternalServices.OpenSky;

public class OpenSkyClient(
    HttpClient httpClient,
    OpenSkyAuthClient authClient,
    IOptions<OpenSkySettings> settings,
    ILogger<OpenSkyClient> logger) : IOpenSkyClient
{
    private static readonly ActivitySource ActivitySource = new("FlightWatch.OpenSky", "1.0.0");

    private readonly HttpClient _httpClient = httpClient;
    private readonly OpenSkyAuthClient _authClient = authClient;
    private readonly OpenSkySettings _settings = settings.Value;
    private readonly ILogger<OpenSkyClient> _logger = logger;

    public async Task<Result<IEnumerable<FlightDto>>> GetAllFlightsAsync()
    {
        using var activity = ActivitySource.StartActivity("GetAllFlights", ActivityKind.Client);
        
        try
        {
            var accessToken = await _authClient.GetAccessTokenAsync();
            
            var requestUri = $"{_settings.BaseUrl}/states/all";
            
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            _logger.LogInformation("Fetching all flights from OpenSky Network");

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("OpenSky token expired, refreshing...");
                _authClient.ClearToken();
                
                accessToken = await _authClient.GetAccessTokenAsync();
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                response = await _httpClient.SendAsync(request);
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "OpenSky API returned error. StatusCode: {StatusCode}",
                    response.StatusCode);

                activity?.SetStatus(ActivityStatusCode.Error, "API error");
                return Result.Failure<IEnumerable<FlightDto>>(
                    Error.ExternalService("OPENSKY_ERROR", $"OpenSky API returned status code: {response.StatusCode}"));
            }

            var content = await response.Content.ReadAsStringAsync();
            var openSkyResponse = JsonSerializer.Deserialize<OpenSkyResponse>(content);

            if (openSkyResponse?.States == null || 
                openSkyResponse.States.Value.ValueKind != System.Text.Json.JsonValueKind.Array ||
                openSkyResponse.States.Value.GetArrayLength() == 0)
            {
                _logger.LogInformation("No flights returned from OpenSky");
                activity?.SetStatus(ActivityStatusCode.Ok);
                return Result.Success(Enumerable.Empty<FlightDto>());
            }

            var stateVectors = openSkyResponse.GetStateVectors();
            var flights = stateVectors
                .Where(sv => sv.Latitude.HasValue && sv.Longitude.HasValue && !sv.OnGround)
                .Select(MapToFlightDto)
                .ToList();

            _logger.LogInformation(
                "Retrieved {Count} flights from OpenSky (filtered from {Total} total states)",
                flights.Count,
                stateVectors.Count);

            activity?.SetTag("flight.count", flights.Count);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return Result.Success<IEnumerable<FlightDto>>(flights);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching flights from OpenSky");
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);
            
            return Result.Failure<IEnumerable<FlightDto>>(
                Error.ExternalService("OPENSKY_ERROR", "Failed to fetch flights from OpenSky Network"));
        }
    }

    public async Task<Result<IEnumerable<FlightDto>>> GetFlightsByBoundingBoxAsync(
        double minLatitude,
        double maxLatitude,
        double minLongitude,
        double maxLongitude)
    {
        using var activity = ActivitySource.StartActivity("GetFlightsByBoundingBox", ActivityKind.Client);
        activity?.SetTag("bbox.min_lat", minLatitude);
        activity?.SetTag("bbox.max_lat", maxLatitude);
        activity?.SetTag("bbox.min_lon", minLongitude);
        activity?.SetTag("bbox.max_lon", maxLongitude);

        try
        {
            var accessToken = await _authClient.GetAccessTokenAsync();

            var requestUri = $"{_settings.BaseUrl}/states/all?" +
                           $"lamin={minLatitude.ToString(CultureInfo.InvariantCulture)}&lamax={maxLatitude.ToString(CultureInfo.InvariantCulture)}" +
                           $"&lomin={minLongitude.ToString(CultureInfo.InvariantCulture)}&lomax={maxLongitude.ToString(CultureInfo.InvariantCulture)}";

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            _logger.LogInformation(
                "Fetching flights from OpenSky in bounding box: ({MinLat},{MinLon}) to ({MaxLat},{MaxLon})",
                minLatitude, minLongitude, maxLatitude, maxLongitude);

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _authClient.ClearToken();
                accessToken = await _authClient.GetAccessTokenAsync();
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                response = await _httpClient.SendAsync(request);
            }

            if (!response.IsSuccessStatusCode)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "API error");
                return Result.Failure<IEnumerable<FlightDto>>(
                    Error.ExternalService("OPENSKY_ERROR", $"OpenSky API returned status code: {response.StatusCode}"));
            }

            var content = await response.Content.ReadAsStringAsync();
            var openSkyResponse = JsonSerializer.Deserialize<OpenSkyResponse>(content);

            if (openSkyResponse?.States == null || 
                openSkyResponse.States.Value.ValueKind != System.Text.Json.JsonValueKind.Array ||
                openSkyResponse.States.Value.GetArrayLength() == 0)
            {
                activity?.SetStatus(ActivityStatusCode.Ok);
                return Result.Success(Enumerable.Empty<FlightDto>());
            }

            var stateVectors = openSkyResponse.GetStateVectors();
            var flights = stateVectors
                .Where(sv => sv.Latitude.HasValue && sv.Longitude.HasValue && !sv.OnGround)
                .Select(MapToFlightDto)
                .ToList();

            activity?.SetTag("flight.count", flights.Count);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return Result.Success<IEnumerable<FlightDto>>(flights);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching flights from OpenSky");
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);

            return Result.Failure<IEnumerable<FlightDto>>(
                Error.ExternalService("OPENSKY_ERROR", "Failed to fetch flights from OpenSky Network"));
        }
    }

    private static FlightDto MapToFlightDto(OpenSkyStateVector state)
    {
        return new FlightDto
        {
            FlightNumber = state.Callsign?.Trim() ?? state.Icao24,
            Airline = state.OriginCountry,
            Latitude = state.Latitude,
            Longitude = state.Longitude,
            Origin = string.Empty,
            Destination = string.Empty,
            Direction = state.TrueTrack,
            FlightStatus = state.OnGround ? "on_ground" : "in_flight",
            Altitude = state.GeoAltitude ?? state.BaroAltitude,
            Velocity = state.Velocity,
            IsLive = true,
            Category = state.Category,
            CategoryDescription = Application.Helpers.AircraftCategoryHelper.GetCategoryShort(state.Category)
        };
    }
}

