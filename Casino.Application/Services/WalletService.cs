using Casino.Core.Entities;
using Casino.Core.ValueObjects;
using Casino.Core.Results;
using Microsoft.Extensions.Logging;
using Casino.Core.Constants;

namespace Casino.Application.Services;

public class WalletService : IWalletService
{
    private readonly IValidationService _validationService;
    private readonly ILogger<WalletService> _logger;

    public WalletService(IValidationService validationService, ILogger<WalletService> logger)
    {
        _validationService = validationService;
        _logger = logger;
    }

    public CommandResult Deposit(Player player, decimal amount)
    {
        try
        {
            // Validate deposit amount
            var validationResult = _validationService.ValidateDepositAmount(amount);
            if (!validationResult.IsValid)
            {
                return CommandResult.Error(validationResult.ErrorMessage);
            }

            // Create money value object
            var depositMoney = new Money(amount);

            // Perform deposit
            var oldBalance = player.Wallet.Balance;
            var newBalance = oldBalance + depositMoney;
            
            player.Wallet.Balance = newBalance;
            
            _logger.LogInformation(LogMessages.DepositSuccessful, 
                depositMoney, newBalance);

            return CommandResult.Success(
                string.Format(UserMessages.DepositSuccessful, depositMoney, newBalance));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, LogMessages.DepositFailed, amount);
            return CommandResult.Error(string.Format(UserMessages.DepositFailed, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogMessages.DepositUnexpectedError, amount);
            return CommandResult.Error(UserMessages.DepositUnexpectedError);
        }
    }

    public CommandResult Withdraw(Player player, decimal amount)
    {
        try
        {
            // Validate withdrawal amount
            var validationResult = _validationService.ValidateWithdrawAmount(amount, player.Wallet.Balance);
            if (!validationResult.IsValid)
            {
                return CommandResult.Error(validationResult.ErrorMessage);
            }

            // Create money value object
            var withdrawMoney = new Money(amount);

            // Check sufficient funds
            if (!HasSufficientFunds(player, amount))
            {
                throw new InvalidOperationException(LogMessages.InsufficientFunds);
            }

            // Perform withdrawal
            var oldBalance = player.Wallet.Balance;
            var newBalance = oldBalance - withdrawMoney;
            
            player.Wallet.Balance = newBalance;

            _logger.LogInformation(LogMessages.WithdrawSuccessful, 
                player.Id, newBalance);

            return CommandResult.Success(
                UserMessages.WithdrawSuccessful,
                newBalance);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, LogMessages.WithdrawFailed, amount);
            return CommandResult.Error(string.Format(UserMessages.WithdrawFailed, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, LogMessages.WithdrawFailed, amount);
            return CommandResult.Error(string.Format(UserMessages.WithdrawFailed, amount));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, LogMessages.WithdrawUnexpectedError, amount);
            return CommandResult.Error(string.Format(UserMessages.WithdrawUnexpectedError, amount));
        }
    }

    public CommandResult PlaceBet(Player player, decimal betAmount)
    {
        try
        {
            // Validate bet amount
            var validationResult = _validationService.ValidateBetAmount(betAmount);
            if (!validationResult.IsValid)
            {
                return CommandResult.Error(validationResult.ErrorMessage);
            }

            // Create bet amount value object
            var betMoney = new BetAmount(betAmount);

            // Check sufficient funds
            if (!HasSufficientFunds(player, betAmount))
            {
                throw new InvalidOperationException(LogMessages.InsufficientFunds);
            }

            // Perform bet
            var oldBalance = player.Wallet.Balance;
            var newBalance = oldBalance - betMoney;
            
            player.Wallet.Balance = newBalance;

            _logger.LogInformation(LogMessages.BetSuccessful, newBalance);

            return CommandResult.Success(UserMessages.BetWin, newBalance);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, LogMessages.BetFailed, betAmount);
            return CommandResult.Error(string.Format(UserMessages.BetFailed, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, LogMessages.BetFailed, betAmount);
            return CommandResult.Error(string.Format(UserMessages.BetFailed, betAmount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogMessages.BetUnexpectedError, betAmount);
            return CommandResult.Error(UserMessages.BetUnexpectedError);
        }
    }

    public CommandResult AcceptWin(Player player, decimal winAmount)
    {
        try
        {
            // Create money value object
            var winMoney = new Money(winAmount);

            // Perform win acceptance
            var oldBalance = player.Wallet.Balance;
            var newBalance = oldBalance + winMoney;
            
            player.Wallet.Balance = newBalance;

            _logger.LogInformation(LogMessages.WinSuccessful, 
                player.Id, winAmount, newBalance);

            return CommandResult.Success(UserMessages.WinSuccessful, newBalance);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, LogMessages.WinFailed, winAmount);
            return CommandResult.Error(string.Format(UserMessages.WinFailed, winAmount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogMessages.WinUnexpectedError, winAmount);
            return CommandResult.Error(string.Format(UserMessages.WinUnexpectedError, winAmount));
        }
    }

    public decimal GetBalance(Player player)
    {
        return player.Wallet.Balance;
    }

    public bool HasSufficientFunds(Player player, decimal amount)
    {
        return player.Wallet.Balance >= amount;
    }
}