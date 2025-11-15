<<<<<<< HEAD
using FlightWatch.Application.Common;
using MediatR;

namespace FlightWatch.Application.Features.FlightMonitoring.Commands.Unsubscribe;

public record UnsubscribeFlightCommand(string ConnectionId) : IRequest<Result>;

=======
using FlightWatch.Application.Common;
using MediatR;

namespace FlightWatch.Application.Features.FlightMonitoring.Commands.Unsubscribe;

public record UnsubscribeFlightCommand(string ConnectionId) : IRequest<Result>;

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
