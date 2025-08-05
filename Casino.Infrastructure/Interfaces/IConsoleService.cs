using Casino.Core.Enums;

namespace Casino.Infrastructure.Interfaces;

public interface IConsoleService
{
    string? GetUserInput(string prompt);
    void DisplayMessage(string message);
    (string Command, decimal Amount) ParseInput(string input);
    CommandType ResolveCommand(string command);
}