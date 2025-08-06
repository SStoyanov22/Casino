namespace Casino.Core.Exceptions;

/// <summary>
/// Thrown when input parsing fails in the console service
/// </summary>
public class ParseInputException : Exception
{
    public ParseInputException(string message) : base(message)
    {
    }
} 