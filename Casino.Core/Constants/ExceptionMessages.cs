namespace Casino.Core.Constants;

public static class ExceptionMessages 
{
    // Money/Amount validation
    public const string AmountMustBePositive = "Amount must be positive";
    public const string MoneyAmountCannotBeNegative = "Money amount cannot be negative";
    public const string BetAmountMustBeBetweenOneAndTen = "Bet amount must be between $1 and $10";
    
    // Wallet operations
    public const string InsufficientFunds = "Insufficient funds";
    public const string InvalidGameResultType = "Invalid game result type";
    
    // Validation
    public const string InvalidConfiguration = "Game configuration is invalid";
    public const string InvalidBetRange = "Bet amount is outside allowed range";
    
}