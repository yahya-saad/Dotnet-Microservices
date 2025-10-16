using CommandsService.AsyncDataService;
using CommandsService.Data;
using CommandsService.EventProcessing;
using CommandsService.Extensions;
using CommandsService.SyncDataServices.grpc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
builder.Services.AddCommandsServices();

builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();
builder.Services.AddHostedService<RabbitMqSubscriber>();

builder.Services.AddControllers();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.PrepPopulation();
app.Run();
