namespace Casino.Application.Services;

public interface IConsoleService
{
    string? GetUserInput(string prompt);
    void DisplayMessage(string message);
    (string Command, decimal Amount) ParseInput(string input);
    bool IsValidCommand(string command);
}