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
                string.Format(UserMessages.DepositSuccessful, amount, newBalance));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex.Message, LogMessages.DepositFailed, amount);
            return CommandResult.Error(
                string.Format(ex.Message + " " + UserMessages.DepositFailed, amount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogMessages.DepositUnexpectedError, amount);
            return CommandResult.Error(
                string.Format(UserMessages.DepositUnexpectedError, amount));
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
                amount, newBalance);

            return CommandResult.Success(
                string.Format(UserMessages.WithdrawSuccessful, amount, newBalance));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex.Message + LogMessages.WithdrawFailed, amount);
            return CommandResult.Error(
                string.Format(ex.Message + " " + UserMessages.WithdrawFailed, amount));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex.Message, LogMessages.WithdrawFailed, amount);
            return CommandResult.Error(
                string.Format(ex.Message + " " + UserMessages.WithdrawFailed, amount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogMessages.WithdrawUnexpectedError, amount);
            return CommandResult.Error(
                string.Format(UserMessages.WithdrawUnexpectedError, amount));
        }
    }

    public CommandResult PlaceBet(Player player, decimal betAmount)
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

            _logger.LogInformation(LogMessages.PlaceBetSuccessful, newBalance);

            return CommandResult.Success(
                string.Format(CultureInfo.InvariantCulture, UserMessages.PlaceBetSuccessful, newBalance));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex.Message, LogMessages.PlaceBetFailed, betAmount);
            return CommandResult.Error(
                string.Format(CultureInfo.InvariantCulture, ex.Message + " " + UserMessages.PlaceBetFailed, betAmount));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex.Message, LogMessages.PlaceBetFailed, betAmount);
            return CommandResult.Error(
                string.Format(CultureInfo.InvariantCulture, ex.Message + " " + UserMessages.PlaceBetFailed, betAmount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogMessages.PlaceBetUnexpectedError, betAmount);
            return CommandResult.Error(
                string.Format(CultureInfo.InvariantCulture, UserMessages.PlaceBetUnexpectedError, betAmount));
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

            _logger.LogInformation(string.Format(CultureInfo.InvariantCulture, LogMessages.AcceptWinSuccessful, newBalance));
            
            return CommandResult.Success(
                string.Format(CultureInfo.InvariantCulture, UserMessages.AcceptWinSuccessful, winAmount, newBalance));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex.Message, LogMessages.AcceptWinFailed, winAmount);
            return CommandResult.Error(
                string.Format(CultureInfo.InvariantCulture, ex.Message + " " + UserMessages.AcceptWinFailed, winAmount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LogMessages.AcceptWinUnexpectedError, winAmount);
            return CommandResult.Error(string.Format(CultureInfo.InvariantCulture, UserMessages.AcceptWinUnexpectedError, winAmount));
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