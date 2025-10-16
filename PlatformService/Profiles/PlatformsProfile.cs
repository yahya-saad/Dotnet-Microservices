using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        CreateMap<Platform, PlatformDto>();
        CreateMap<PlatformCreateDto, Platform>();
        CreateMap<PlatformDto, PlatformPublishedDto>();

        // gRPC
        CreateMap<Platform, GrpcPlatformModel>()
            .ForMember(dest => dest.PlatformId, opt => opt.MapFrom(src => src.Id));
    }
}
