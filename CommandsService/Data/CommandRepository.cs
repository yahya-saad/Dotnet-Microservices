using CommandsService.Models;

namespace CommandsService.Data;

public class CommandRepository : ICommandRepository
{
    private readonly AppDbContext _context;

    public CommandRepository(AppDbContext context)
    {
        _context = context;
    }

    public void CreateCommand(int platformId, Command command)
    {
        command.PlatformId = platformId;
        _context.Commands.Add(command);
    }

    public void DeleteCommand(Command command) => _context.Commands.Remove(command);

    public IEnumerable<Command> GetAllCommandsForPlatform(int platformId) =>
        _context.Commands.Where(c => c.PlatformId == platformId).ToList();


    public Command? GetCommand(int platformId, int commandId) =>
        _context.Commands.FirstOrDefault(c => c.PlatformId == platformId && c.Id == commandId);

}
