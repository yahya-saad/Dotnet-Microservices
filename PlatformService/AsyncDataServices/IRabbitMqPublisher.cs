namespace PlatformService.AsyncDataServices;

public interface IRabbitMqPublisher
{
    Task PublishMessage<T>(T message);
    bool IsConnected { get; }
}
