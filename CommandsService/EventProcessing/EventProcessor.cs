using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using System.Text.Json;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;
    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }
    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);
        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event");
        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
        return eventType?.Event switch
        {
            "Platform_Published" => EventType.PlatformPublished,
            _ => EventType.Undetermined
        };
    }

    private void AddPlatform(string platformPublishedMessage)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepositoryManager>();
        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

        var platform = _mapper.Map<Platform>(platformPublishedDto);
        if (!repository.Platform.ExternalPlatformExists(platform.ExternalId))
        {
            repository.Platform.CreatePlatform(platform);
            repository.Save();
            Console.WriteLine("--> Platform added!");
        }
        else
        {
            Console.WriteLine("--> Platform already exists...");
        }
    }
}


enum EventType
{
    PlatformPublished,
    Undetermined
}
