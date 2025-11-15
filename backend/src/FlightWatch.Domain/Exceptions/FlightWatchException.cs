namespace FlightWatch.Domain.Exceptions;

public abstract class FlightWatchException : Exception
{
    protected FlightWatchException(string message) : base(message)
    {
    }

    protected FlightWatchException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

