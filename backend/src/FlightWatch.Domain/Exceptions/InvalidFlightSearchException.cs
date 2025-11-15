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

