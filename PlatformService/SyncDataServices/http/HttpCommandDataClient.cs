using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _httpClient;

    public HttpCommandDataClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendPlatformToCommand(PlatformDto platform)
    {
        var response = await _httpClient.PostAsJsonAsync("api/c/platforms", platform);
        response.EnsureSuccessStatusCode();

        if (response.IsSuccessStatusCode)
            Console.WriteLine("--> Sync POST to CommandService was OK!");
        else
            Console.WriteLine("--> Sync POST to CommandService was NOT OK!");
    }
}
