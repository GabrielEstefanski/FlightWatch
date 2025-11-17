using FlightWatch.Application.Interfaces;
using FlightWatch.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FlightWatch.Api.Tests.Helpers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RemoveMongoDbServices(this IServiceCollection services)
    {
        var mongoClientDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IMongoClient));
        if (mongoClientDescriptor != null)
        {
            services.Remove(mongoClientDescriptor);
        }

        var mongoDbContextDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(MongoDbContext));
        if (mongoDbContextDescriptor != null)
        {
            services.Remove(mongoDbContextDescriptor);
        }

        var userRepositoryDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IUserRepository));
        if (userRepositoryDescriptor != null)
        {
            services.Remove(userRepositoryDescriptor);
        }

        var refreshTokenRepositoryDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IRefreshTokenRepository));
        if (refreshTokenRepositoryDescriptor != null)
        {
            services.Remove(refreshTokenRepositoryDescriptor);
        }

        var flightSubscriptionRepositoryDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IFlightSubscriptionRepository));
        if (flightSubscriptionRepositoryDescriptor != null)
        {
            services.Remove(flightSubscriptionRepositoryDescriptor);
        }

        var eventStoreDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IEventStore));
        if (eventStoreDescriptor != null)
        {
            services.Remove(eventStoreDescriptor);
        }

        return services;
    }

    public static IServiceCollection AddTestRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IRefreshTokenRepository, InMemoryRefreshTokenRepository>();
        services.AddSingleton<IFlightSubscriptionRepository, InMemoryFlightSubscriptionRepository>();
        services.AddSingleton<IEventStore, InMemoryEventStore>();

        return services;
    }
}

