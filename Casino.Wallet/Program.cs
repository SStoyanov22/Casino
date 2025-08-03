using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Casino.Core.Configurations;
using Casino.Application.Services;
using Casino.Core.Entities;

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
        services.AddScoped<ISlotGameService, SlotGameService>();
        services.AddScoped<ICommandHandler, CommandHandler>();
        services.AddScoped<IValidationService, ValidationService>();

        // Build service provider
        var serviceProvider = services.BuildServiceProvider();

        // Get logger
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Casino Application Starting...");
        
        // Main application logic here

        var player = new Player();
        var commandHandler = serviceProvider.GetRequiredService<ICommandHandler>();

        await RunApplicationAsync(player, commandHandler, logger);

        logger.LogInformation("Casino Application Shutdown");

        
    }

    private static async Task RunApplicationAsync(Player player, ICommandHandler commandHandler, ILogger<Program> logger)
    {
        Console.WriteLine("Welcome to our Casino!");
        Console.WriteLine("Available commands: deposit <amount>, withdraw <amount>, bet <amount>, exit");
        Console.WriteLine();

        while (true)
        {
            try
            {
                var input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input))
                    {
                        Console.WriteLine("Please enter a valid command.");
                        continue;
                    }

                var command = input.Split(' ');
                if (command.Length == 0)
                {
                    Console.WriteLine("Please enter a valid command.");
                    continue;
                }

                // Parse and execute command

               // Display result

                
                // Check if user wants to exit
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in main application loop");
                Console.WriteLine("An unexpected error occurred. Please try again.");
                Console.WriteLine();
            }
        }

    }
}
