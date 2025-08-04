using Microsoft.Extensions.Logging;

namespace Casino.Application.Services;

public class ConsoleService : IConsoleService
{
    private readonly ILogger<ConsoleService> _logger;
    private readonly string[] _validCommands = { "deposit", "withdraw", "bet", "exit" };

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
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input))
            {
                _logger.LogWarning("Input is empty");
                return null;
            }
            return input;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading user input");
            return null;
        }
    }

    public bool IsValidCommand(string command)
    {
        return _validCommands.Contains(command.ToLowerInvariant());
    }

    public (string Command, decimal Amount) ParseInput(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            _logger.LogWarning("Input is empty");
            return ("invalid", 0);
        }

        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            _logger.LogWarning("Invalid input format");
            return ("invalid", 0);
        }
        var command = parts[0].ToLowerInvariant();
        decimal amount = 0m;

        if (parts.Length > 1)
        {
            if (!decimal.TryParse(parts[1], out amount))
            {
                _logger.LogWarning("Invalid amount format");
                return ("invalid", 0);
            }
        }
        
        _logger.LogDebug("Parsed input - Command: {Command}, Amount: {Amount}", command, amount);
        return (command, amount);
    }
}
