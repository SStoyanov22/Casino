using Microsoft.Extensions.Options;
using Casino.Core.Configurations;
using Casino.Core.Constants;
using Casino.Core.Results;
using Casino.Infrastructure.Interfaces;

namespace Casino.Infrastructure.Services;

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
            _ => ValidationResult.Error(UserMessages.UnknownOperation)
        };
    }

    public ValidationResult ValidateBetAmount(decimal amount)
    {
        if (amount <= 0)
        {
            return ValidationResult.Error(UserMessages.BetAmountMustBeGreaterThanZero);
        }

        if (amount < _gameConfig.MinimumBet || amount > _gameConfig.MaximumBet)
        {
                return ValidationResult.Error(
                    string.Format(UserMessages.BetAmountOutMustBeBetween, _gameConfig.MinimumBet, _gameConfig.MaximumBet));
        }


        return ValidationResult.Success();
    }

    public ValidationResult ValidateDepositAmount(decimal amount)
    {
        if (amount <= 0)
        {
            return ValidationResult.Error(UserMessages.DepositAmountMustBeGreaterThanZero);
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidateWithdrawAmount(decimal amount, decimal balance)
    {
        if (amount <= 0)
        {
            return ValidationResult.Error(UserMessages.WithdrawAmountMustBeGreaterThanZero);
        }

        if (amount > balance)
        {
            return ValidationResult.Error(UserMessages.WithdrawAmountMustBeLessThanBalance);
        }

        return ValidationResult.Success();
    }
}