using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    public PlatformsController(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    [HttpGet]
    [EndpointSummary("Get all platforms")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetPlatforms()
    {
        Console.WriteLine("--> Getting Platforms from CommandsService");

        var platforms = _repository.Platform.GetAllPlatforms();
        return Ok(_mapper.Map<IEnumerable<PlatformDto>>(platforms));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Get a specific platform by ID")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPlatformById(int id)
    {
        Console.WriteLine($"--> Getting Platform {id} from CommandsService");

        var platform = _repository.Platform.GetPlatformById(id);
        if (platform == null)
            return NotFound();

        var platformDto = _mapper.Map<PlatformDto>(platform);

        return Ok(platformDto);
    }

    [HttpPost("test-inbound")]
    public IActionResult TestInboundConnection()
    {
        Console.WriteLine("--> Inbound [POST] #CommandService");
        return Ok("[Platforms Controller] Inbound test");
    }
}
