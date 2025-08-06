using Microsoft.Extensions.Options;
using Casino.Core.Configurations;
using Casino.Core.Constants;
using Casino.Core.Results;
using Casino.Infrastructure.Interfaces;
using Casino.Core.Enums;

namespace Casino.Infrastructure.Services;

public class ValidationService : IValidationService
{
    private readonly GameConfiguration _gameConfig;

    public ValidationService(IOptions<GameConfiguration> gameConfig)
    {
        _gameConfig = gameConfig.Value;
        GameConfiguration.Initialize(_gameConfig); // Initialize static instance
    }
    public ValidationResult ValidateAmount(CommandType commandType, decimal amount, decimal? balance = null)
    {
        return commandType switch
        {
            CommandType.Deposit => ValidateDepositAmount(amount),
            CommandType.Withdraw => ValidateWithdrawAmount(amount, balance ?? 0), // Balance should be provided for withdraw
            CommandType.Bet => ValidateBetAmount(amount),
            _ => ValidationResult.Error(UserMessages.UnknownOperation)
        };
    }

    private ValidationResult ValidateBetAmount(decimal amount)
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

    private ValidationResult ValidateDepositAmount(decimal amount)
    {
        if (amount <= 0)
        {
            return ValidationResult.Error(UserMessages.DepositAmountMustBeGreaterThanZero);
        }

        return ValidationResult.Success();
    }

    private ValidationResult ValidateWithdrawAmount(decimal amount, decimal balance)
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