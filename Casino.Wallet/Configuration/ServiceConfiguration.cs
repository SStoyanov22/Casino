using Casino.Application.Commands;
using Casino.Application.Engines;
using Casino.Core.Commands;
using Casino.Core.Configurations;
using Casino.Core.Results;
using Casino.Infrastructure.Interfaces;
using Casino.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Casino.Wallet.Configuration;

public static class ServiceConfiguration
{
    public static IServiceProvider ConfigureServices()
    {
        var configuration = BuildConfiguration();
        var services = new ServiceCollection();
        
        RegisterConfiguration(services, configuration);
        RegisterLogging(services);
        RegisterApplicationServices(services);
        RegisterCommands(services);
        
        return services.BuildServiceProvider();
    }

    
        private static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
        
        private static void RegisterConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConfiguration>(configuration);
            services.Configure<GameConfiguration>(configuration.GetSection("GameConfiguration"));
        }
        
        private static void RegisterLogging(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
        }
        
        private static void RegisterApplicationServices(IServiceCollection services)
        {
            services.AddScoped<ISlotGameService, SlotGameService>();
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IConsoleService, ConsoleService>();
            services.AddScoped<IGameEngine, GameEngine>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IApplicationService, ApplicationService>();

        services.AddSingleton<IRandomNumberGeneratorService, RandomNumberGeneratorService>();
        }
        
        private static void RegisterCommands(IServiceCollection services)
        {
            services.AddScoped<ICommand<CommandResult>, DepositCommand>();
            services.AddScoped<ICommand<CommandResult>, WithdrawCommand>();
            services.AddScoped<ICommand<CommandResult>, BetCommand>();
            services.AddScoped<ICommand<CommandResult>, ExitCommand>();
        }
}