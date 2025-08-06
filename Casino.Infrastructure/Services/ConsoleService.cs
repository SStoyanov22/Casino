using Casino.Core.Constants;
using Casino.Core.Enums;
using Casino.Core.Exceptions;
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
            _logger.LogError(ex.Message, LogMessages.ErrorReadingUserInput);

            return null;
        }
    }

    public (string Command, decimal? Amount) ParseInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            _logger.LogWarning(LogMessages.InputEmpty);
            throw new ParseInputException(LogMessages.InputEmpty);
        }

        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            _logger.LogWarning(LogMessages.EmptyInputAfterSplitting);
            throw new ParseInputException(LogMessages.EmptyInputAfterSplitting);
        }

        var command = parts[0].ToLowerInvariant();

        // Commands that don't require amounts
        if (IsCommandWithoutAmount(command))
        {
            _logger.LogDebug(string.Format(LogMessages.ParsedCommandWithoutAmount, command));
            return (command, null);
        }

        // Commands that require amounts
        if (parts.Length != 2)
        {
            _logger.LogWarning(LogMessages.CommandRequiresAmount, command);
            throw new ParseInputException(string.Format(LogMessages.CommandRequiresAmount, command));
        }

        if (!decimal.TryParse(parts[1], out decimal amount))
        {
            _logger.LogWarning(LogMessages.CommandInvalidAmountFormat, command, parts[1]);
            throw new ParseInputException(string.Format(LogMessages.CommandInvalidAmountFormat, command, parts[1]));
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
            _ => throw new ArgumentException(LogMessages.InvalidCommand, command)
        };

        return commandType;
    }

    private static bool IsCommandWithoutAmount(string command)
    {
        return command == "exit" ? true : false;
    }
}
