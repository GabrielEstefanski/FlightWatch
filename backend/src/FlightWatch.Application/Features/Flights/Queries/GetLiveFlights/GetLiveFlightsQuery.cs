using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs;
using MediatR;

namespace FlightWatch.Application.Features.Flights.Queries.GetLiveFlights;

public record GetLiveFlightsQuery : IRequest<Result<IEnumerable<FlightDto>>>;
