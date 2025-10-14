using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.Profiles;

public class CommandProfile : Profile
{
    public CommandProfile()
    {
        CreateMap<Command, CommandDto>();
        CreateMap<CommandCreateDto, Command>();
    }
}
