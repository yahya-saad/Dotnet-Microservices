using CommandsService.Models;
using CommandsService.SyncDataServices.grpc;

namespace CommandsService.Data;

public static class PrebareDatabase
{
    public static void PrepPopulation(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var repositoryManager = scope.ServiceProvider.GetRequiredService<IRepositoryManager>();

        var grpcClient = scope.ServiceProvider.GetRequiredService<IPlatformDataClient>();
        var platforms = grpcClient.ReturnAllPlatforms();

        SeedData(repositoryManager, platforms);

    }

    private static void SeedData(IRepositoryManager repositoryManager, IEnumerable<Platform> platforms)
    {
        Console.WriteLine("[gRPC] Seeding platforms");

        foreach (var platform in platforms)
        {
            if (!repositoryManager.Platform.ExternalPlatformExists(platform.ExternalId))
            {
                repositoryManager.Platform.CreatePlatform(platform);
                repositoryManager.Save();
            }
        }
    }
}
