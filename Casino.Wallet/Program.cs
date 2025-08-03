using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Casino.Core.Configurations;

namespace Casino.Wallet;

class Program
{
    static async Task Main(string[] args)
    {
        // Setup configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Setup dependency injection
        var services = new ServiceCollection();
        
        // Register configuration
        services.AddSingleton<IConfiguration>(configuration);
        
        // Register logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Register game configuration
        services.Configure<GameConfiguration>(
            configuration.GetSection("GameConfiguration"));

        // Register services
        services.AddScoped<IGameEngine, GameEngine>();
        services.AddScoped<ICommandHandler, CommandHandler>();

        // Build service provider
        var serviceProvider = services.BuildServiceProvider();

        // Get logger
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Betty Casino Application Starting...");
        
        // TODO: Add main application logic here
        Console.WriteLine("Welcome to Betty Casino!");
        Console.WriteLine("Application is ready for implementation.");
        
        logger.LogInformation("Casino Application Started Successfully");
    }
}
