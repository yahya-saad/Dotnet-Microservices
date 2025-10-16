using PlatformService.SyncDataServices.grpc;

namespace PlatformService.Extentions;

public static class GrpcApplicationExtensions
{
    public static WebApplication MapGrpcEndpoints(this WebApplication app)
    {
        app.MapGrpcService<GrpcPlatformService>();

        app.MapGet("/protos/platforms.proto", async context =>
        {
            var protoPath = Path.Combine(AppContext.BaseDirectory, "Protos", "platforms.proto");

            if (!File.Exists(protoPath))
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Proto file not found.");
                return;
            }

            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(await File.ReadAllTextAsync(protoPath));
        });

        return app;
    }
}