using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Casino.Application.Services;
using Casino.Core.Entities;
using Casino.Wallet.Configuration;
using Casino.Application.Engines;
using Casino.Core.Constants;
using System.Globalization;

namespace Casino.Wallet;

class Program
{
    static async Task Main(string[] args)
    {
        // Force USD currency formatting
        var usCulture = CultureInfo.GetCultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = usCulture;
        CultureInfo.DefaultThreadCurrentUICulture = usCulture;
        Thread.CurrentThread.CurrentCulture = usCulture;
        Thread.CurrentThread.CurrentUICulture = usCulture;
        
        // Configure all services
        var serviceProvider = ServiceConfiguration.ConfigureServices();

        // Get required services
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation(LogMessages.ApplicationStarting);
        
        try
        {
            var gameEngine = serviceProvider.GetRequiredService<IGameEngine>();
            await gameEngine.StartAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, LogMessages.FatalErrorOccurred);
        }
        finally
        {
            logger.LogInformation(LogMessages.ApplicationShutdown);
            if (serviceProvider is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
