<<<<<<< HEAD
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

=======
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

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
