using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.grpc;

public class GrpcPlatformService : GrpcPlatforms.GrpcPlatformsBase
{
    private readonly IMapper mapper;
    private readonly IPlatformRepository platformRepository;
    private readonly ILogger<GrpcPlatformService> logger;

    public GrpcPlatformService(IPlatformRepository platformRepository, IMapper mapper, ILogger<GrpcPlatformService> logger)
    {
        this.platformRepository = platformRepository;
        this.mapper = mapper;
        this.logger = logger;
    }

    public override Task<GetAllPlatformsResponse> GetAllPlatforms(EmptyRequest request, ServerCallContext context)
    {
        logger.LogInformation("--> {Method} invoked via gRPC", nameof(GetAllPlatforms));
        var platforms = platformRepository.GetAllPlatforms();

        var response = new GetAllPlatformsResponse();
        var grpcPlatforms = mapper.Map<IEnumerable<GrpcPlatformModel>>(platforms);
        response.Platforms.AddRange(grpcPlatforms);

        return Task.FromResult(response);
    }
}
