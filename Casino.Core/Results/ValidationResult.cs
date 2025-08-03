namespace Casino.Core.Results;

public class ValidationResult
{
    public bool IsValid { get; }
    public string? ErrorMessage { get; }

    private ValidationResult(bool isValid, string? errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success()
    {
        return new ValidationResult(true);
    }

    public static ValidationResult Error(string message)
    {
        return new ValidationResult(false, message);
    }
}