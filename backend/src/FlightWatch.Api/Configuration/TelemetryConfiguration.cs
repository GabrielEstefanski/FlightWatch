using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FlightWatch.Api.Configuration;

public static class TelemetryConfiguration
{
    private const string ServiceName = "FlightWatch.Api";
    private const string ServiceVersion = "1.0.0";

    public static IServiceCollection AddTelemetry(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: ServiceName,
                    serviceVersion: ServiceVersion)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development",
                    ["host.name"] = Environment.MachineName
                }))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.EnrichWithHttpRequest = (activity, httpRequest) =>
                    {
                        activity.SetTag("http.request.header.user_agent", httpRequest.Headers.UserAgent.ToString());
                        activity.SetTag("http.request.header.host", httpRequest.Host.ToString());
                    };
                    options.EnrichWithHttpResponse = (activity, httpResponse) =>
                    {
                        activity.SetTag("http.response.status_code", httpResponse.StatusCode);
                    };
                })
                .AddHttpClientInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.EnrichWithHttpRequestMessage = (activity, httpRequestMessage) =>
                    {
                        activity.SetTag("http.request.method", httpRequestMessage.Method.ToString());
                        activity.SetTag("http.request.uri", httpRequestMessage.RequestUri?.ToString());
                    };
                    options.EnrichWithHttpResponseMessage = (activity, httpResponseMessage) =>
                    {
                        activity.SetTag("http.response.status_code", (int)httpResponseMessage.StatusCode);
                    };
                })
                .AddSource("FlightWatch.*")
                .AddConsoleExporter());

        return services;
    }
}
