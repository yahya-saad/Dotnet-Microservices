using CommandsService.Models;

namespace CommandsService.SyncDataServices.grpc;

public interface IPlatformDataClient
{
    IEnumerable<Platform> ReturnAllPlatforms();
}
