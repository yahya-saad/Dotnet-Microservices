using PlatformService.Models;

namespace PlatformService.Data;

public static class PlatformDataSeeder
{
    public static void SeedPlatforms(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        SeedDatabase(context);
    }

    private static void SeedDatabase(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Platforms.Any())
        {
            Console.WriteLine("--> Seeding initial platform data...");
            context.Platforms.AddRange(GetPreconfiguredPlatforms());
            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> Platforms data already exists.");
        }
    }


    private static IEnumerable<Platform> GetPreconfiguredPlatforms()
    {
        return new List<Platform>
    {
        new() { Name = ".NET", Publisher = "Microsoft", Cost = "Free" },
        new() { Name = "Node.js", Publisher = "OpenJS Foundation", Cost = "Free" },
        new() { Name = "Python", Publisher = "Python Software Foundation", Cost = "Free" },
        new() { Name = "Java", Publisher = "Oracle", Cost = "Free" },
        new() { Name = "Docker", Publisher = "Docker Inc.", Cost = "Free" },
        new() { Name = "Kubernetes", Publisher = "CNCF", Cost = "Free" },
        new() { Name = "MySQL", Publisher = "Oracle", Cost = "Free" },
        new() { Name = "PostgreSQL", Publisher = "PostgreSQL Global Development Group", Cost = "Free" },
        new() { Name = "MongoDB", Publisher = "MongoDB Inc.", Cost = "Free (Community)" },
        new() { Name = "Redis", Publisher = "Redis Ltd.", Cost = "Free (Open Source)" }
    };
    }
}
