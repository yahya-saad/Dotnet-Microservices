namespace CommandsService.Data;

public class RepositoryManager : IRepositoryManager
{
    private readonly AppDbContext _context;
    private readonly Lazy<IPlatformRepository> _platformRepository;
    private readonly Lazy<ICommandRepository> _commandRepository;

    public RepositoryManager(AppDbContext context)
    {
        _context = context;
        _platformRepository = new Lazy<IPlatformRepository>(() => new PlatformRepository(_context));
        _commandRepository = new Lazy<ICommandRepository>(() => new CommandRepository(_context));
    }

    public IPlatformRepository Platform => _platformRepository.Value;
    public ICommandRepository Command => _commandRepository.Value;

    public bool Save() => (_context.SaveChanges() > 0);
}
