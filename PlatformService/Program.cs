using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Extentions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration, builder.Environment);

builder.Services.AddPlatformServices(builder.Configuration);

builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

builder.Services.AddControllers();
builder.Services.AddGrpc();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.SeedPlatforms();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcEndpoints();

app.Run();
