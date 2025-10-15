using Microsoft.OpenApi.Models;
using PlatformService.SyncDataServices.http;

namespace PlatformService.Extentions;

public static class ServiceExtensions
{
    public static IServiceCollection AddPlatformServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureSwagger();
        services.ConfigureAutoMapper();
        services.ConfigureHttpClient(configuration);
        return services;
    }

    private static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
        });
    }

    private static void ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));
    }

    private static void ConfigureHttpClient(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>(c =>
        {
            c.BaseAddress = new Uri(configuration["CommandService:BaseUrl"]!);
        })
            .ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(10)
            };
        }).SetHandlerLifetime(Timeout.InfiniteTimeSpan);
    }
}
