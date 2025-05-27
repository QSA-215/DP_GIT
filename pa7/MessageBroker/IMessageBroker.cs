namespace MessageBroker;

public interface IMessageBroker
{
    public Task SendMessageAsync(string exchangeName, string message, string routingKey = "");
    public Task ReceiveMessageAsync(string queueName, Action<string> messageHandler);
}