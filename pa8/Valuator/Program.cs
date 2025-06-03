using DBController;
using MessageBroker;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Valuator;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Login";
                options.LogoutPath = "/Logout";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            });
        builder.Services.AddSingleton<IDBController, RedisDB>();

        string exchangeName = "valuator.processing.rank";
        string queueName = "valuator.processing.rank";
        string similarityRoutingKey = "SimilarityCalculated";

        RabbitMqService messageBroker = await RabbitMqService.CreateAsync("rabbitmq", "rabbituser", "rabbitpassword");
        await messageBroker.DeclareTopologyAsync(exchangeName, queueName);
        await messageBroker.DeclareTopologyAsync(exchangeName, queueName, similarityRoutingKey);

        builder.Services.AddSingleton<IMessageBroker>(_ => messageBroker);

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();
        app.UseAuthentication();

        app.MapRazorPages();

        app.Run();
    }
}