using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRepository : IPlatformRepository
{
    private readonly AppDbContext _context;
    public PlatformRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Platform> GetAllPlatforms() => _context.Platforms.ToList();

    public Platform? GetPlatformById(int id) => _context.Platforms.FirstOrDefault(p => p.Id == id) ?? null;

    public void CreatePlatform(Platform platform) => _context.Platforms.Add(platform);
    public void DeletePlatform(Platform platform) => _context.Platforms.Remove(platform);
    public bool SaveChanges() => (_context.SaveChanges() >= 0);


}
