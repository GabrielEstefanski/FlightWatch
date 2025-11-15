using AutoMapper;
using FlightWatch.Application.DTOs.FlightMonitoring;
using FlightWatch.Domain.Entities;

namespace FlightWatch.Application.Mappings;

public class FlightMonitoringMappingProfile : Profile
{
    public FlightMonitoringMappingProfile()
    {
        CreateMap<FlightSubscription, SubscriptionDto>();
        CreateMap<FlightUpdate, FlightUpdateDto>();
    }
}

