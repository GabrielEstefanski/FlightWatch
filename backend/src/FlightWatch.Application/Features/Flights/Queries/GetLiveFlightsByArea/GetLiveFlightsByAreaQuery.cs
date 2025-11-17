using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs;
using MediatR;

namespace FlightWatch.Application.Features.Flights.Queries.GetLiveFlightsByArea;

public record GetLiveFlightsByAreaQuery(
    double MinLatitude,
    double MaxLatitude,
    double MinLongitude,
    double MaxLongitude) : IRequest<Result<IEnumerable<FlightDto>>>;
