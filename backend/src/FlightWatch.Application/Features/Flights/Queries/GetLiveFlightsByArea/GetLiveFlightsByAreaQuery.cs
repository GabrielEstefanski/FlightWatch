<<<<<<< HEAD
using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs;
using MediatR;

namespace FlightWatch.Application.Features.Flights.Queries.GetLiveFlightsByArea;

public record GetLiveFlightsByAreaQuery(
    double MinLatitude,
    double MaxLatitude,
    double MinLongitude,
    double MaxLongitude) : IRequest<Result<IEnumerable<FlightDto>>>;

=======
using FlightWatch.Application.Common;
using FlightWatch.Application.DTOs;
using MediatR;

namespace FlightWatch.Application.Features.Flights.Queries.GetLiveFlightsByArea;

public record GetLiveFlightsByAreaQuery(
    double MinLatitude,
    double MaxLatitude,
    double MinLongitude,
    double MaxLongitude) : IRequest<Result<IEnumerable<FlightDto>>>;

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
