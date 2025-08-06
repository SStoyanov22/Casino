using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Casino.Wallet.Configuration;
using Casino.Application.Engines;
using Casino.Core.Constants;
using System.Globalization;

namespace Casino.Wallet;

class Program
{
    static async Task Main(string[] args)
    {
        // Set global culture for consistent USD formatting
        var usCulture = CultureInfo.GetCultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = usCulture;
        CultureInfo.DefaultThreadCurrentUICulture = usCulture;
        
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
            logger.LogError(ex.Message, LogMessages.FatalErrorOccurred);
        }
        finally
        {
            logger.LogInformation(LogMessages.ApplicationShutdown);
            if (serviceProvider is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
