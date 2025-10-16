using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.grpc;

public class PlatformDataClient : IPlatformDataClient
{
    private readonly ILogger<PlatformDataClient> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public PlatformDataClient(ILogger<PlatformDataClient> logger, IConfiguration configuration, IMapper mapper)
    {
        _logger = logger;
        _configuration = configuration;
        _mapper = mapper;
    }


    public IEnumerable<Platform> ReturnAllPlatforms()
    {
        _logger.LogInformation("Calling gRPC Service to get all Platforms...");

        try
        {
            var grpcAddress = _configuration["GrpcPlatform"];
            if (string.IsNullOrWhiteSpace(grpcAddress))
            {
                _logger.LogWarning("GrpcPlatform address not configured.");
                return Enumerable.Empty<Platform>();
            }

            var handler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true,
                AutomaticDecompression = System.Net.DecompressionMethods.None
            };

            using var channel = GrpcChannel.ForAddress(grpcAddress, new GrpcChannelOptions
            {
                HttpHandler = handler,
                Credentials = Grpc.Core.ChannelCredentials.Insecure

            });
            var client = new GrpcPlatforms.GrpcPlatformsClient(channel);

            var reply = client.GetAllPlatforms(new EmptyRequest());

            _logger.LogInformation("Retrieved {Count} platform(s) from gRPC.", reply.Platforms.Count);

            return _mapper.Map<IEnumerable<Platform>>(reply.Platforms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling gRPC server.");
            return Enumerable.Empty<Platform>();
        }
    }

}
