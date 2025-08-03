namespace Casino.Core.Constants;

public static class ExceptionMessages 
{
    // Money/Amount validation
    public const string AmountMustBePositive = "Amount must be positive";
    public const string MoneyAmountCannotBeNegative = "Money cannot be negative";

    public const string DepositAmountMustBeGreaterThanZero = "Deposit amount must be greater than zero";
    public const string DepositAmountMustBeGreaterThanMinimumAllowed = "Deposit amount must be greater than minimum allowed";
    public const string DepositAmountMustBeLessThanMaximumAllowed = "Deposit amount must be less than maximum allowed";
    public const string WithdrawAmountMustBeGreaterThanZero = "Withdraw amount must be greater than zero";
    public const string WithdrawAmountMustBeLessThanBalance = "Withdraw amount must be less than balance";
    public const string WithdrawAmountMustBeLessThanMaximumAllowed = "Withdraw amount must be less than maximum allowed";
    public const string WithdrawAmountMustBeGreaterThanMinimumAllowed = "Withdraw amount must be greater than minimum allowed";
    public const string BetAmountMustBeGreaterThanZero = "Bet amount must be greater than zero";
    public const string BetAmountMustBeLessThanBalance = "Bet amount must be less than balance";
    public const string BetAmountMustBeLessThanMaximumAllowed = "Bet amount must be less than maximum allowed";
    public const string BetAmountMustBeGreaterThanMinimumAllowed = "Bet amount must be greater than minimum allowed";
    
    // Wallet operations
    public const string InsufficientFunds = "Insufficient funds";
    public const string InvalidGameResultType = "Invalid game result type";
    
    // Validation
    public const string InvalidConfiguration = "Game configuration is invalid";
    public const string InvalidBetRange = "Bet amount is outside allowed range";
    
    // 

}