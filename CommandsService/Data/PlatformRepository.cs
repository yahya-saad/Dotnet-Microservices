using CommandsService.Models;

namespace CommandsService.Data;

public class PlatformRepository : IPlatformRepository
{
    private readonly AppDbContext _context;

    public PlatformRepository(AppDbContext context)
    {
        _context = context;
    }

    public void CreatePlatform(Platform platform) => _context.Platforms.Add(platform);

    public IEnumerable<Platform> GetAllPlatforms() => _context.Platforms.ToList();

    public Platform? GetPlatformById(int id) => _context.Platforms.FirstOrDefault(p => p.Id == id);

    public bool PlatformExists(int platformId) => _context.Platforms.Any(p => p.Id == platformId);
    public bool ExternalPlatformExists(int externalPlatformId) =>
        _context.Platforms.Any(p => p.ExternalId == externalPlatformId);

}
