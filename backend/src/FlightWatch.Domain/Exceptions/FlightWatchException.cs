<<<<<<< HEAD
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

=======
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

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
