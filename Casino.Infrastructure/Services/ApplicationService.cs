using Casino.Core.Constants;
using Casino.Core.DTOs;
using Casino.Core.Entities;
using Casino.Core.Enums;
using Casino.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Casino.Infrastructure.Services;

public class ApplicationService : IApplicationService
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IConsoleService _consoleService;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(
        ICommandDispatcher commandDispatcher,
         IConsoleService consoleService,
          ILogger<ApplicationService> logger)
    {
        _commandDispatcher = commandDispatcher;
        _consoleService = consoleService;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        var player = new Player(); // Starts with $0 balance
    
        // Show welcome and instructions
        _consoleService.DisplayMessage(UserMessages.Welcome);
        _consoleService.DisplayMessage(UserMessages.AvailableCommands);
        _consoleService.DisplayMessage(""); // Empty line
        while (true)
        {
            try
            {
                var input = _consoleService.GetUserInput(UserMessages.PromptAction);
                if (string.IsNullOrEmpty(input))
                    {
                        _consoleService.DisplayMessage(UserMessages.PleaseEnterValidCommand);
                        continue;
                    }

                // Parse input
                var (command, amount) = _consoleService.ParseInput(input);

                // Parse command and create request
                CommandType commandType;
                try
                {
                    commandType = _consoleService.ResolveCommand(command);
                }
                catch (ArgumentException ex)
                {
                    _logger.LogWarning(ex.Message, LogMessages.InvalidCommand, command);
                    _consoleService.DisplayMessage(UserMessages.InvalidCommand);
                    continue;
                }

                var request = new CommandRequest(amount.Value, player);

                // Dispatch command
                var result = await _commandDispatcher.DispatchAsync(commandType, request);
                
                // Display result
                _consoleService.DisplayMessage(result.Message);

                // Check if user wants to exit
                if (commandType == CommandType.Exit)
                {
                    break;
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, UserMessages.UnexpectedErrorInMainLoop);
                _consoleService.DisplayMessage(UserMessages.UnexpectedErrorInMainLoop);
            }
        }
    }
}