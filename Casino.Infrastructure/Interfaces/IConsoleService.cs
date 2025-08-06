using Casino.Core.Enums;

namespace Casino.Infrastructure.Interfaces;

public interface IConsoleService
{
    string? GetUserInput(string prompt);
    void DisplayMessage(string message);
    (CommandType commandType, decimal? amount) ParseInput(string input);
}