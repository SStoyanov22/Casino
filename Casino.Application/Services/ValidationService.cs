
using Casino.Core.Configurations;
using Casino.Core.Constants;
using Casino.Core.Results;
using Microsoft.Extensions.Options;

namespace Casino.Application.Services;

public class ValidationService : IValidationService
{
    private readonly GameConfiguration _gameConfig;

    public ValidationService(IOptions<GameConfiguration> gameConfig)
    {
        _gameConfig = gameConfig.Value;
        GameConfiguration.Initialize(_gameConfig); // Initialize static instance
    }
    public ValidationResult ValidateAmount(decimal amount, string operation)
    {
        return operation.ToLowerInvariant() switch
        {
            "deposit" => ValidateDepositAmount(amount),
            "withdraw" => ValidateWithdrawAmount(amount, 0), // Balance will be checked in command
            "bet" => ValidateBetAmount(amount),
            _ => ValidationResult.Error($"Unknown operation: {operation}")
        };
    }

    public ValidationResult ValidateBetAmount(decimal amount)
    {
        if (amount <= 0)
        {
            return ValidationResult.Error(ExceptionMessages.BetAmountMustBeGreaterThanZero);
        }

        if (!_gameConfig.IsValidBetAmount(amount))
        {
            return ValidationResult.Error(ExceptionMessages.BetAmountMustBeLessThanMaximumAllowed);
        }

        if (amount < _gameConfig.MinimumBet)
        {
            return ValidationResult.Error(ExceptionMessages.BetAmountMustBeGreaterThanMinimumAllowed);
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidateDepositAmount(decimal amount)
    {
        if (amount <= 0)
        {
            return ValidationResult.Error(ExceptionMessages.DepositAmountMustBeGreaterThanZero);
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidateWithdrawAmount(decimal amount, decimal balance)
    {
        if (amount <= 0)
        {
            return ValidationResult.Error(ExceptionMessages.WithdrawAmountMustBeGreaterThanZero);
        }

        if (amount > balance)
        {
            return ValidationResult.Error(ExceptionMessages.WithdrawAmountMustBeLessThanBalance);
        }

        return ValidationResult.Success();
    }
}