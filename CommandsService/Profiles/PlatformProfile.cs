using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.Profiles;

public class PlatformProfile : Profile
{
    public PlatformProfile()
    {
        CreateMap<Platform, PlatformDto>();
        CreateMap<PlatformPublishedDto, Platform>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(p => p.Id));

        // gRPC
        CreateMap<GrpcPlatformModel, Platform>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(p => p.PlatformId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(p => p.Name))
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
