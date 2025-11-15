namespace FlightWatch.Domain.Exceptions;

public class AuthenticationException : FlightWatchException
{
    public AuthenticationException(string message) : base(message)
    {
    }

    public AuthenticationException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

