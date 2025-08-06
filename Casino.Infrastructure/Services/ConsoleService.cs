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

    public (CommandType commandType, decimal? amount) ParseInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ParseInputException(LogMessages.InputEmpty);
        }

        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            throw new ParseInputException(LogMessages.EmptyInputAfterSplitting);
        }

        var command = parts[0];
        
        if (parts.Length > 2)
        {
            throw new ArgumentException(LogMessages.InputTooLong);
        }

        decimal amount = 0;
        var commandType = ResolveCommand(command);

        if (parts.Length == 2 &&
            !decimal.TryParse(parts[1], out amount))
        {
            throw new ParseInputException(string.Format(LogMessages.CommandInvalidAmountFormat, command, parts[1]));
        }
         else if (parts.Length == 2 && 
            decimal.TryParse(parts[1], out amount))
        {
            if(commandType != CommandType.Exit)
            {
                _logger.LogInformation(LogMessages.ParsedInput, command, amount);

                return (commandType, amount);
            }
            else
            {
                throw new ArgumentException(string.Format(LogMessages.CommandWithoutAmount, command));
            }
        }
        else {
            _logger.LogInformation(LogMessages.ParsedInputWithOneArgument, command);
            return (commandType, null);
        }
    }

    private static CommandType ResolveCommand(string command)
    {
        var commandType = command.Trim().ToLowerInvariant() switch
        {
            "deposit" => CommandType.Deposit,
            "withdraw" => CommandType.Withdraw,
            "bet" => CommandType.Bet,
            "exit" => CommandType.Exit,
            _ => throw new InvalidCommandException(string.Format(LogMessages.InvalidCommand, command))
        };

        return commandType;
    }
}
