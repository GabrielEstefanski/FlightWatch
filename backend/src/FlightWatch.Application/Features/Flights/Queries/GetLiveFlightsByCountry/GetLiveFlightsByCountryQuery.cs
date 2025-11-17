using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs;
using MediatR;

namespace FlightWatch.Application.Features.Flights.Queries.GetLiveFlightsByCountry;

public record GetLiveFlightsByCountryQuery(string Country) : IRequest<Result<IEnumerable<FlightDto>>>;

