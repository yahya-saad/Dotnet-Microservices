using CommandsService.Models;

namespace CommandsService.Data;

public interface ICommandRepository
{
    IEnumerable<Command> GetAllCommandsForPlatform(int platformId);
    Command? GetCommand(int platformId, int commandId);
    void CreateCommand(int platformId, Command command);
}
