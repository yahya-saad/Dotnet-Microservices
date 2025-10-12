using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.http;

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformDto platform);
}
