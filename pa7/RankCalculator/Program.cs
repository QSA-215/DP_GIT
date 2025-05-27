using DBController;
using MessageBroker;
using RankCalculator;

internal class Program
{
    private static async Task Main(string[] args)
    {

        string hostname = "rabbitmq";
        string rankCalculatorQueueName = "valuator.processing.rank";
        string rankCalculatorExchangeName = "valuator.processing.rank";
        string eventLoggerQueueName = "events.loger";
        string eventLoggerExhangeName = "events.loger";
        string rankRoutingKey = "RankCalculated";

        RabbitMqService messageBroker = await RabbitMqService.CreateAsync(hostname);
        await messageBroker.DeclareTopologyAsync(rankCalculatorExchangeName, rankCalculatorQueueName);
        await messageBroker.DeclareTopologyAsync(eventLoggerQueueName, eventLoggerExhangeName, rankRoutingKey);
        RankCalculatorService rankCalculatorService = new(new RedisDB(), messageBroker);
        await messageBroker.ReceiveMessageAsync(rankCalculatorQueueName, async message => await rankCalculatorService.Proccess(message));

        var exitEvent = new TaskCompletionSource<bool>();
        Console.CancelKeyPress += async (sender, args) =>
        {
            Console.WriteLine("Stopping rankCalculator");
            await messageBroker.DisposeAsync();
            args.Cancel = true;
            exitEvent.SetResult(true);
        };
        await exitEvent.Task;

        Console.WriteLine("RankCalculator stoped");
    }
}