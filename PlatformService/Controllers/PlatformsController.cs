using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.http;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepository platformRepository;
    private readonly IMapper mapper;
    private readonly ICommandDataClient commandDataClient;
    private readonly IRabbitMqPublisher _rabbitMqPublisher;
    public PlatformsController(IPlatformRepository platformRepository, IMapper mapper, ICommandDataClient commandDataClient, IRabbitMqPublisher rabbitMqPublisher)
    {
        this.platformRepository = platformRepository;
        this.mapper = mapper;
        this.commandDataClient = commandDataClient;
        _rabbitMqPublisher = rabbitMqPublisher;
    }

    [HttpGet]
    [EndpointSummary("Get all platforms")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<PlatformDto>> GetPlatforms()
    {
        var host = Environment.GetEnvironmentVariable("HOSTNAME") ?? "unknown";
        Console.WriteLine($"--> Host {host} responding from PlatformService");

        var platforms = platformRepository.GetAllPlatforms();
        var result = mapper.Map<IEnumerable<PlatformDto>>(platforms);
        return Ok(platforms);
    }


    [HttpGet("{id:int}")]
    [EndpointSummary("Get platform by id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PlatformDto> GetPlatformById(int id)
    {
        var platform = platformRepository.GetPlatformById(id);
        if (platform == null)
            return NotFound();

        var result = mapper.Map<PlatformDto>(platform);
        return Ok(result);
    }

    [HttpPost]
    [EndpointSummary("Create a new platform")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PlatformDto>> CreatePlatform([FromBody] PlatformCreateDto dto)
    {
        var platform = mapper.Map<Platform>(dto);
        platformRepository.CreatePlatform(platform);
        platformRepository.SaveChanges();

        var result = mapper.Map<PlatformDto>(platform);

        // Send sync Message
        try
        {
            await commandDataClient.SendPlatformToCommand(result);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
        }

        // Send Async Message via RabbitMQ
        try
        {
            var platformsPublishDto = mapper.Map<PlatformPublishedDto>(result);
            await _rabbitMqPublisher.PublishMessage(platformsPublishDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
        }

        return CreatedAtAction(nameof(GetPlatformById), new { id = platform.Id }, result);
    }

    [HttpDelete("{id:int}")]
    [EndpointSummary("Delete a platform by id")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult DeletePlatform(int id)
    {
        var platform = platformRepository.GetPlatformById(id);
        if (platform == null)
            return NotFound();

        platformRepository.DeletePlatform(platform);
        platformRepository.SaveChanges();

        return NoContent();
    }

}
