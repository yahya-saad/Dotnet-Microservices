using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CommandsService.AsyncDataService;

public class RabbitMqSubscriber : BackgroundService, IAsyncDisposable
{
    private readonly ILogger<RabbitMqSubscriber> _logger;
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private readonly string _exchangeName = "trigger";

    private IConnection? _connection;
    private IChannel? _channel;
    private string? _queueName;
    private bool _disposed;

    public bool IsConnected => _connection?.IsOpen == true && _channel?.IsOpen == true;

    public RabbitMqSubscriber(ILogger<RabbitMqSubscriber> logger, IConfiguration configuration, IEventProcessor eventProcessor)
    {
        _logger = logger;
        _configuration = configuration;
        _eventProcessor = eventProcessor;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await InitializeConnectionAsync();

        if (!IsConnected)
        {
            _logger.LogError("Failed to connect to RabbitMQ, subscriber will not start");
            return;
        }

        await StartConsumingAsync(cancellationToken);
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

            var queueDeclareResult = await _channel.QueueDeclareAsync();
            _queueName = queueDeclareResult.QueueName;

            await _channel.QueueBindAsync(queue: _queueName, exchange: _exchangeName, routingKey: string.Empty);

            _connection.ConnectionShutdownAsync += OnConnectionShutdown;
            _connection.ConnectionRecoveryErrorAsync += OnConnectionRecoveryError;
            _connection.RecoverySucceededAsync += OnRecoverySucceeded;

            _logger.LogInformation("Successfully connected to RabbitMQ at {Host}:{Port} with queue {QueueName}",
                factory.HostName, factory.Port, _queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ Message Bus");
        }
    }

    private async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        if (_channel == null || _queueName == null)
        {
            _logger.LogError("Channel or QueueName is null, cannot start consuming messages");
            return;
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received message: {Message}", message);

                _eventProcessor.ProcessEvent(message);

                // Acknowledge the message
                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _queueName,
            autoAck: false, // Manual
            consumer: consumer);

        _logger.LogInformation("Started consuming messages from queue {QueueName}", _queueName);

        // Keep the service running until cancellation is requested
        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Message consumption cancelled");
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

        _logger.LogInformation("RabbitMQ subscriber disposed");
    }
}
