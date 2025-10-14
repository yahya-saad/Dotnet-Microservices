namespace CommandsService.Models;
public class Command
{
    public int Id { get; set; }
    public string HowTo { get; set; } = string.Empty;
    public string CommandLine { get; set; } = string.Empty;
    public int PlatformId { get; set; }
    public Platform Platform { get; set; } = null!;
}
