using System.Diagnostics;
using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs;
using FlightWatch.Application.Features.Flights.Queries.GetLiveFlights;
using FlightWatch.Application.Features.Flights.Queries.GetLiveFlightsByArea;
using FlightWatch.Application.Features.Flights.Queries.GetLiveFlightsByCountry;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightWatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightsController(IMediator mediator, ILogger<FlightsController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<FlightsController> _logger = logger;

    [HttpGet("live")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FlightDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLiveFlights()
    {
        var query = new GetLiveFlightsQuery();
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<IEnumerable<FlightDto>>.FailureResponse(
                new ErrorDetails
                {
                    Code = result.Error.Code,
                    Message = result.Error.Message,
                    Type = result.Error.Type.ToString()
                },
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));
        }

        return Ok(ApiResponse<IEnumerable<FlightDto>>.SuccessResponse(
            result.Value,
            Activity.Current?.Id ?? HttpContext.TraceIdentifier));
    }

    [HttpGet("live/area")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FlightDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLiveFlightsByArea(
        [FromQuery] double minLat,
        [FromQuery] double maxLat,
        [FromQuery] double minLon,
        [FromQuery] double maxLon)
    {
        var query = new GetLiveFlightsByAreaQuery(minLat, maxLat, minLon, maxLon);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<IEnumerable<FlightDto>>.FailureResponse(
                new ErrorDetails
                {
                    Code = result.Error.Code,
                    Message = result.Error.Message,
                    Type = result.Error.Type.ToString()
                },
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));
        }

        return Ok(ApiResponse<IEnumerable<FlightDto>>.SuccessResponse(
            result.Value,
            Activity.Current?.Id ?? HttpContext.TraceIdentifier));
    }

    [HttpGet("live/country/{country}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FlightDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLiveFlightsByCountry(string country)
    {
        var query = new GetLiveFlightsByCountryQuery(country);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<IEnumerable<FlightDto>>.FailureResponse(
                new ErrorDetails
                {
                    Code = result.Error.Code,
                    Message = result.Error.Message,
                    Type = result.Error.Type.ToString()
                },
                Activity.Current?.Id ?? HttpContext.TraceIdentifier));
        }

        return Ok(ApiResponse<IEnumerable<FlightDto>>.SuccessResponse(
            result.Value,
            Activity.Current?.Id ?? HttpContext.TraceIdentifier));
    }
}

