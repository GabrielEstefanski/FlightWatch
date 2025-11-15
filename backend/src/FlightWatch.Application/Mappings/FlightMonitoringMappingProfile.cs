<<<<<<< HEAD
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

=======
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

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
