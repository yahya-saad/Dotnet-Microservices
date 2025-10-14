using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/platforms/{platformId:int}/[controller]")]
[ApiController]

public class CommandsController : ControllerBase
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    public CommandsController(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    [EndpointSummary("Get all commands for a platform")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CommandDto> GetCommandsForPlatform(int platformId)
    {
        Console.WriteLine($"--> Getting Commands for Platform {platformId}");
        var platformExists = _repository.Platform.PlatformExists(platformId);
        if (!platformExists)
            return NotFound();

        var commands = _repository.Command.GetAllCommandsForPlatform(platformId);
        var commandsDto = _mapper.Map<IEnumerable<CommandDto>>(commands);

        return Ok(commandsDto);
    }

    [HttpGet("{commandId:int}")]
    [EndpointSummary("Get a specific command for a platform")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CommandDto> GetCommandForPlatform(int platformId, int commandId)
    {
        Console.WriteLine($"--> Getting Command {commandId} for Platform {platformId}");
        var platformExists = _repository.Platform.PlatformExists(platformId);
        if (!platformExists)
            return NotFound();

        var command = _repository.Command.GetCommand(platformId, commandId);
        if (command == null)
            return NotFound();

        var commandDto = _mapper.Map<CommandDto>(command);

        return Ok(commandDto);
    }

    [HttpPost]
    [EndpointSummary("Create a command for a platform")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CommandDto> CreateCommandForPlatform(int platformId, [FromBody] CommandCreateDto commandCreateDto)
    {
        Console.WriteLine($"--> Creating Command for Platform {platformId}");
        var platformExists = _repository.Platform.PlatformExists(platformId);
        if (!platformExists)
            return NotFound();

        var command = _mapper.Map<Command>(commandCreateDto);
        _repository.Command.CreateCommand(platformId, command);
        _repository.Save();

        var commandDto = _mapper.Map<CommandDto>(command);

        return CreatedAtAction(nameof(GetCommandForPlatform),
            new { platformId = platformId, commandId = commandDto.Id }, commandDto);
    }

}
