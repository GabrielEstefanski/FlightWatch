namespace FlightWatch.Domain.Exceptions;

public class InvalidTokenException : AuthenticationException
{
    public InvalidTokenException(string message) : base(message)
    {
    }

    public InvalidTokenException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

