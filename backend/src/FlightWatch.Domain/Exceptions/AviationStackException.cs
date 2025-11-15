namespace FlightWatch.Domain.Exceptions;

public class AviationStackException : ExternalServiceException
{
    public AviationStackException(string message) 
        : base("AviationStack", message)
    {
    }

    public AviationStackException(string message, int statusCode) 
        : base("AviationStack", message, statusCode)
    {
    }

    public AviationStackException(string message, Exception innerException) 
        : base("AviationStack", message, innerException)
    {
    }
}

