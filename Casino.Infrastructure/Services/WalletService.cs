using Casino.Core.Entities;
using Casino.Core.ValueObjects;
using Casino.Core.Results;
using Microsoft.Extensions.Logging;
using Casino.Core.Constants;
using Casino.Infrastructure.Interfaces;
using System.Globalization;

namespace Casino.Infrastructure.Services;

public class WalletService : IWalletService
{
    private readonly ILogger<WalletService> _logger;

    public WalletService(ILogger<WalletService> logger)
    {
        _logger = logger;
    }

    public CommandResult Deposit(Player player, decimal amount)
    {
        try
        {
            // Create money value object (validation done at command level)
            var depositMoney = new Money(amount);

            // Perform deposit
            var oldBalance = player.Wallet.Balance;
            var newBalance = oldBalance + depositMoney;
            
            player.Wallet.Balance = newBalance;
            
            _logger.LogInformation(LogMessages.DepositSuccessful, 
                amount, newBalance);

            return CommandResult.Success(
                string.Format(CultureInfo.InvariantCulture, UserMessages.DepositSuccessful, amount, newBalance));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex.Message, LogMessages.DepositFailed, amount);
            return CommandResult.Error(
                string.Format(UserMessages.DepositFailed, amount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, LogMessages.DepositUnexpectedError, amount);
            return CommandResult.Error(
                string.Format(UserMessages.DepositFailed, amount));
        }
    }

    public CommandResult Withdraw(Player player, decimal amount)
    {
        try
        {
            // Create money value object (validation done at command level)
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
                amount, (decimal)newBalance);

            return CommandResult.Success(
                string.Format(UserMessages.WithdrawSuccessful, amount, (decimal)newBalance));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex.Message, LogMessages.WithdrawFailed, amount);
            return CommandResult.Error(
                string.Format(UserMessages.WithdrawFailed, amount));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex.Message, LogMessages.WithdrawFailed, amount);
            return CommandResult.Error(
                string.Format(UserMessages.WithdrawFailed, amount));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex.Message, LogMessages.WithdrawUnexpectedError, amount);
            return CommandResult.Error(
                string.Format(UserMessages.WithdrawFailed, amount));
        }
    }

    public CommandResult AcceptLoss(Player player, decimal betAmount)
    {
        try
        {
            // Create bet amount value object (validation done at command level)
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

            _logger.LogInformation(LogMessages.AcceptLossSuccessful, (decimal)newBalance);

            return CommandResult.Success(
                string.Format(UserMessages.AcceptLossSuccessful, (decimal)newBalance));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex.Message, LogMessages.AcceptLossFailed, betAmount);
            return CommandResult.Error(
                string.Format(UserMessages.AcceptLossFailed, betAmount));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex.Message, LogMessages.AcceptLossFailed, betAmount);
            return CommandResult.Error(
                string.Format(UserMessages.AcceptLossFailed, betAmount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, LogMessages.AcceptLossUnexpectedError, betAmount);
            return CommandResult.Error(
                string.Format(UserMessages.AcceptLossFailed, betAmount));
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

            _logger.LogInformation(LogMessages.AcceptWinSuccessful, (decimal)newBalance);
            
            return CommandResult.Success(
                string.Format(UserMessages.AcceptWinSuccessful, winAmount, (decimal)newBalance));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex.Message, LogMessages.AcceptWinFailed, winAmount);
            return CommandResult.Error(
                string.Format(UserMessages.AcceptWinFailed, winAmount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, LogMessages.AcceptWinUnexpectedError, winAmount);
            return CommandResult.Error(string.Format(UserMessages.AcceptWinFailed, winAmount));
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