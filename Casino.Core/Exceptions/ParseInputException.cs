namespace Casino.Core.Exceptions;

/// <summary>
/// Thrown when input parsing fails in the console service
/// </summary>
public class ParseInputException : Exception
{
    public string Input { get; }

    public ParseInputException(string message) : base(message)
    {
        Input = string.Empty;
    }

    public ParseInputException(string input, string message) : base(message)
    {
        Input = input;
    }

    public ParseInputException(string input, string message, Exception innerException) : base(message, innerException)
    {
        Input = input;
    }
} 