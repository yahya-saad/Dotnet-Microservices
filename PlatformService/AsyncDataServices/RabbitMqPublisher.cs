namespace PlatformService.AsyncDataServices;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class RabbitMqPublisher : IRabbitMqPublisher, IAsyncDisposable
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly string _exchangeName = "trigger";

    private IConnection? _connection;
    private IChannel? _channel;
    private bool _disposed;

    public bool IsConnected => _connection?.IsOpen == true && _channel?.IsOpen == true;

    public RabbitMqPublisher(IConfiguration configuration, ILogger<RabbitMqPublisher> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _ = Task.Run(InitializeConnectionAsync);
    }

    private async Task InitializeConnectionAsync()
    {
        try
        {
            if (_disposed || IsConnected) return;

            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:Username"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest",
                AutomaticRecoveryEnabled = true,
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Fanout);

            _connection.ConnectionShutdownAsync += OnConnectionShutdown;
            _connection.ConnectionRecoveryErrorAsync += OnConnectionRecoveryError;
            _connection.RecoverySucceededAsync += OnRecoverySucceeded;

            _logger.LogInformation("Successfully connected to RabbitMQ at {Host}:{Port}",
                factory.HostName, factory.Port);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ Message Bus");
        }
    }

    public async Task PublishMessage<T>(T message)
    {
        if (_disposed || message == null) return;
        if (!IsConnected) return;

        try
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            await _channel!.BasicPublishAsync(_exchangeName, "", body);
            _logger.LogInformation("Message of type {MessageType} published", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message of type {MessageType}", typeof(T).Name);
        }
    }
    // Event handlers for connection events
    private Task OnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        _logger.LogWarning("RabbitMQ connection shutdown: {Reason}", e.ReplyText);
        return Task.CompletedTask;
    }

    private Task OnConnectionRecoveryError(object? sender, ConnectionRecoveryErrorEventArgs e)
    {
        _logger.LogError(e.Exception, "RabbitMQ connection recovery failed");
        return Task.CompletedTask;
    }

    private Task OnRecoverySucceeded(object? sender, AsyncEventArgs e)
    {
        _logger.LogInformation("RabbitMQ connection recovery succeeded");
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        if (_channel != null) await _channel.DisposeAsync();
        if (_connection != null) await _connection.DisposeAsync();
    }
}