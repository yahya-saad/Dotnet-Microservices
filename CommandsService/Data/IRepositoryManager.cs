namespace CommandsService.Data;

public interface IRepositoryManager
{
    IPlatformRepository Platform { get; }
    ICommandRepository Command { get; }
    bool Save();
}
