using Microsoft.EntityFrameworkCore;
using PlatformService.Data;

namespace PlatformService.Extentions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            Console.WriteLine("Using InMemory DB");
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseInMemoryDatabase("InMemory"));
        }
        else
        {
            Console.WriteLine("Using SQL Server DB");
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }

        return services;
    }
}
