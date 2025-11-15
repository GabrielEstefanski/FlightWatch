using AutoMapper;
using FlightWatch.Application.DTOs;
using FlightWatch.Application.Helpers;
using FlightWatch.Domain.Entities;

namespace FlightWatch.Application.Mappings;

public class FlightMappingProfile : Profile
{
    public FlightMappingProfile()
    {
        CreateMap<Flight, FlightDto>()
            .ForMember(dest => dest.CategoryDescription, 
                opt => opt.MapFrom(src => AircraftCategoryHelper.GetCategoryShort(src.Category)));
    }
}

