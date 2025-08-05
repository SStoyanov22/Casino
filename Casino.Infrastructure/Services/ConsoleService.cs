using Casino.Core.Constants;
using Casino.Core.Enums;
using Casino.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Casino.Infrastructure.Services;

public class ConsoleService : IConsoleService
{
    private readonly ILogger<ConsoleService> _logger;

    public ConsoleService(ILogger<ConsoleService> logger)
    {
        _logger = logger;
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }

    public string? GetUserInput(string prompt)
    {
        try
        {
            var input = Console.ReadLine()?.Trim();
            
            return input;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogMessages.ErrorReadingUserInput);

            return null;
        }
    }

    public (string Command, decimal Amount) ParseInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            _logger.LogWarning(LogMessages.InputEmpty);
            return ("invalid", 0);
        }

        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            _logger.LogWarning(LogMessages.EmptyInputAfterSplitting);
            return ("invalid", 0);
        }

        var command = parts[0].ToLowerInvariant();
        decimal amount = 0m;

        // Commands that don't require amounts
        if (IsCommandWithoutAmount(command))
        {
            _logger.LogDebug(LogMessages.ParsedCommandWithoutAmount, command);
            return (command, 0);
        }

        // Commands that require amounts
        if (parts.Length != 2)
        {
            _logger.LogWarning(LogMessages.CommandRequiresAmount, command);
            return ("invalid", 0);
        }

        if (!decimal.TryParse(parts[1], out amount))
        {
            _logger.LogWarning(LogMessages.CommandInvalidAmountFormat, command, parts[1]);
            return ("invalid", 0);
        }

        _logger.LogDebug(LogMessages.ParsedInput, command, amount);
        return (command, amount);
    }

    public CommandType ResolveCommand(string command)
    {
        var commandType = command.Trim().ToLowerInvariant() switch
        {
            "deposit" => CommandType.Deposit,
            "withdraw" => CommandType.Withdraw,
            "bet" => CommandType.Bet,
            "exit" => CommandType.Exit,
            _ => throw new ArgumentException(LogMessages.InvalidCommand)
        };

        return commandType;
    }

    private static bool IsCommandWithoutAmount(string command)
    {
        return command == "exit" ? true : false;
    }
}
