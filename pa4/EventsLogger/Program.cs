using MessageBroker;

internal class Program
{
    private static async Task Main(string[] args)
    {
        string eventLoggerQueueName = "events.loger";
        string eventLoggerExchangeName = "events.loger";
        string rankRoutingKey = "RankCalculated";
        string similarityRoutingKey = "SimilarityCalculated";
        RabbitMqService messageBroker = await RabbitMqService.CreateAsync("rabbitmq");
        await messageBroker.DeclareTopologyAsync(eventLoggerQueueName, eventLoggerExchangeName, rankRoutingKey);
        await messageBroker.DeclareTopologyAsync(eventLoggerQueueName, eventLoggerExchangeName, similarityRoutingKey);

        await messageBroker.ReceiveMessageAsync(eventLoggerQueueName,
            message =>
            {
                Console.WriteLine("Update data:");
                Console.WriteLine(message);
            }
        );

        TaskCompletionSource<bool> exitEvent = new();
        Console.CancelKeyPress += (sender, args) =>
        {
            args.Cancel = true;
            exitEvent.SetResult(true);
        };
        await exitEvent.Task;

    }
}