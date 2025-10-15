namespace PlatformService.Dtos;

public record PlatformPublishedDto(
    int Id,
    string Name,
    string Event = "Platform_Published"
);

