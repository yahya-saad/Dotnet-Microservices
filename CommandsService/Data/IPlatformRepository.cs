using CommandsService.Models;

namespace CommandsService.Data;

public interface IPlatformRepository
{
    IEnumerable<Platform> GetAllPlatforms();
    Platform? GetPlatformById(int id);
    void CreatePlatform(Platform platform);
    bool PlatformExists(int platformId);
}
