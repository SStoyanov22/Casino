using Casino.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Casino.Application.Services;

public class ApplicationService : IApplicationService
{
    private readonly ICommandHandler _commandHandler;
    private readonly IConsoleService _consoleService;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(
        ICommandHandler commandHandler,
         IConsoleService consoleService,
          ILogger<ApplicationService> logger)
    {
        _commandHandler = commandHandler;
        _consoleService = consoleService;
        _logger = logger;
    }

    public Task RunAsync()
    {
        var player = new Player();

        while (true)
        {
            while (true)
        {
            try
            {
                var input = _consoleService.GetUserInput("Please, submit action: ");
                if (string.IsNullOrEmpty(input))
                    {
                        _consoleService.DisplayMessage("Please enter a valid command.");
                        continue;
                    }

                // Parse input
                var (command, amount) = _consoleService.ParseInput(input);
                
                if (!_consoleService.IsValidCommand(command))
                {
                    _consoleService.DisplayMessage($"Invalid command: {command}");
                    continue;
                }

                // Execute command
                var result = await _commandHandler.ExecuteCommandAsync(command, amount, player, commandHandler);
                
                // Display result
                _consoleService.DisplayMessage(result.Message);

                // Check if user wants to exit
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in main application loop");
                Console.WriteLine("An unexpected error occurred. Please try again.");
                Console.WriteLine();
            }
        }
        }
    }
}