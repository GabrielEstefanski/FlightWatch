<<<<<<< HEAD
namespace FlightWatch.Domain.Exceptions;

public class InvalidFlightSearchException : FlightWatchException
{
    public string? Origin { get; }
    public string? Destination { get; }

    public InvalidFlightSearchException(string message) : base(message)
    {
    }

    public InvalidFlightSearchException(string message, string? origin, string? destination) 
        : base(message)
    {
        Origin = origin;
        Destination = destination;
    }
}

=======
namespace FlightWatch.Domain.Exceptions;

public class InvalidFlightSearchException : FlightWatchException
{
    public string? Origin { get; }
    public string? Destination { get; }

    public InvalidFlightSearchException(string message) : base(message)
    {
    }

    public InvalidFlightSearchException(string message, string? origin, string? destination) 
        : base(message)
    {
        Origin = origin;
        Destination = destination;
    }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
