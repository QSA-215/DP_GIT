using MessageBroker;
using StackExchange.Redis;

namespace Valuator;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddSingleton<IConnectionMultiplexer>(options => ConnectionMultiplexer.Connect("redis:6379"));

        string hostname = "rabbitmq";
        string exchangeName = "valuator.processing.rank";
        string queueName = "valuator.processing.rank";
        string similarityRoutingKey = "SimilarityCalculated";//!!!

        RabbitMqService messageBroker = await RabbitMqService.CreateAsync(hostname);
        await messageBroker.DeclareTopologyAsync(exchangeName, queueName);

        await messageBroker.DeclareTopologyAsync(exchangeName, queueName, similarityRoutingKey);//!!!

        builder.Services.AddSingleton<IMessageBroker>(_ => messageBroker);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}