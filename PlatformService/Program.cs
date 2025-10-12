using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Extentions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMemeory"));
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

builder.Services.AddControllers();
builder.Services.AddPlatformServices();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.SeedPlatforms();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () =>
{
    var podName = Environment.GetEnvironmentVariable("HOSTNAME");
    return $"Hello from {podName}";
});

app.Run();
