using FlightWatch.Application.Configuration;
using FlightWatch.Application.Events;
using FlightWatch.Application.Interfaces;
using FlightWatch.Infrastructure.EventHandlers;
using FlightWatch.Infrastructure.Messaging;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightWatch.Infrastructure.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddEventSourcing(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMQ"));
        services.AddScoped<IEventBus, RabbitMqEventBus>();

        var rabbitMqSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>() ?? new RabbitMqSettings();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<FlightSubscriptionCreatedEventHandler>();
            x.AddConsumer<FlightDataUpdatedEventHandler>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                cfg.UseMessageRetry(r => 
                {
                    r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
                });

                cfg.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });

                cfg.UseRateLimit(1000, TimeSpan.FromSeconds(1));

                cfg.ConfigureEndpoints(context);

                cfg.Message<FlightSubscriptionCreatedIntegrationEvent>(e => 
                    e.SetEntityName("flight-subscription-created"));
                
                cfg.Message<FlightDataUpdatedIntegrationEvent>(e => 
                    e.SetEntityName("flight-data-updated"));
            });
        });

        return services;
    }
}

