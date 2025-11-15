<<<<<<< HEAD
namespace FlightWatch.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public ErrorDetails? Error { get; set; }
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string? traceId = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            TraceId = traceId,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse<T> FailureResponse(ErrorDetails error, string? traceId = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = error,
            TraceId = traceId,
            Timestamp = DateTime.UtcNow
        };
    }
}

public class ErrorDetails
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Type { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}

=======
namespace FlightWatch.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public ErrorDetails? Error { get; set; }
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string? traceId = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            TraceId = traceId,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse<T> FailureResponse(ErrorDetails error, string? traceId = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = error,
            TraceId = traceId,
            Timestamp = DateTime.UtcNow
        };
    }
}

public class ErrorDetails
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Type { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
