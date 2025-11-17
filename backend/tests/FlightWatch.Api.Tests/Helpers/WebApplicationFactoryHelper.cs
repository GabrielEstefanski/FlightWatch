using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FlightWatch.Api.Tests.Helpers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "MongoDB:ConnectionString", "mongodb://localhost:27017/flightwatch_test" },
                { "MongoDB:DatabaseName", "flightwatch_test" },
                { "JwtSettings:SecretKey", "TestSecretKeyForJWTTokenGeneration12345678901234567890" },
                { "JwtSettings:Issuer", "FlightWatch.Test" },
                { "JwtSettings:Audience", "FlightWatch.Test" },
                { "JwtSettings:ExpirationMinutes", "60" },
                { "RabbitMQ:Host", "localhost" },
                { "RabbitMQ:Port", "5672" },
                { "RabbitMQ:Username", "guest" },
                { "RabbitMQ:Password", "guest" },
                { "RabbitMQ:VirtualHost", "/" },
                { "OpenSky:BaseUrl", "https://opensky-network.org/api" },
                { "OpenSky:ClientId", "test_client" },
                { "OpenSky:ClientSecret", "test_secret" },
                { "ASPNETCORE_ENVIRONMENT", "Testing" }
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveMongoDbServices();
            services.AddTestRepositories();

            var backgroundServiceDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(Microsoft.Extensions.Hosting.IHostedService) &&
                                     d.ImplementationType?.Name == "FlightUpdateBackgroundService");

            if (backgroundServiceDescriptor != null)
            {
                services.Remove(backgroundServiceDescriptor);
            }
        });

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Warning);
        });
    }
}

