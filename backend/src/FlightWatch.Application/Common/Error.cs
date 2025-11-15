<<<<<<< HEAD
namespace FlightWatch.Application.Common;

public sealed class Error
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }

    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);

    public static Error Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    public static Error ExternalService(string code, string message) =>
        new(code, message, ErrorType.ExternalService);

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
}

public enum ErrorType
{
    None = 0,
    Failure = 1,
    Validation = 2,
    NotFound = 3,
    ExternalService = 4
}

=======
namespace FlightWatch.Application.Common;

public sealed class Error
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }

    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);

    public static Error Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    public static Error ExternalService(string code, string message) =>
        new(code, message, ErrorType.ExternalService);

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
}

public enum ErrorType
{
    None = 0,
    Failure = 1,
    Validation = 2,
    NotFound = 3,
    ExternalService = 4
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
