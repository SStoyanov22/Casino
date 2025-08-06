namespace Casino.Core.Exceptions;

/// <summary>
/// Exception thrown when an invalid command is provided
/// </summary>
public class InvalidCommandException : Exception
{
    public InvalidCommandException(string message) : base(message)
    {
    }
}