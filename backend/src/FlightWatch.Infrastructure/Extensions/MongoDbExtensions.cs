using FlightWatch.Application.Interfaces;
using FlightWatch.Infrastructure.Configuration;
using FlightWatch.Infrastructure.Data;
using FlightWatch.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FlightWatch.Infrastructure.Extensions;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        if (services.Any(s => s.ServiceType == typeof(IUserRepository)))
        {
            return services;
        }

        var environment = configuration["ASPNETCORE_ENVIRONMENT"];
        if (environment == "Testing")
        {
            return services;
        }

        var mongoDbSettings = configuration.GetSection("MongoDB").Get<MongoDbSettings>();
        if (mongoDbSettings == null || string.IsNullOrEmpty(mongoDbSettings.ConnectionString))
        {
            throw new InvalidOperationException("MongoDB configuration is required. Please set MongoDB__ConnectionString and MongoDB__DatabaseName in configuration.");
        }

        var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
        services.AddSingleton<IMongoClient>(mongoClient);

        _ = services.AddScoped<MongoDbContext>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return new MongoDbContext(client, mongoDbSettings.DatabaseName);
        });

        services.AddRepositories();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IFlightSubscriptionRepository, FlightSubscriptionRepository>();
        services.AddScoped<IEventStore, EventStore.EventStore>();

        return services;
    }
}

