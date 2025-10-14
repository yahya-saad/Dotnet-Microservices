namespace CommandsService.Models;

public class Platform
{
    public int Id { get; set; }
    public int ExternalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Command> Commands { get; set; } = new List<Command>();
}
