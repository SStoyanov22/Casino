using Casino.Core.Entities;
using Casino.Core.ValueObjects;
using Casino.Core.Results;
using Microsoft.Extensions.Logging;

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
            
            _logger.LogInformation("Deposit successful for player {PlayerId}. New balance: {NewBalance}", 
                player.Id, newBalance);

            return CommandResult.Success(
                $"Your deposit of ${amount:F2} was successful. Your current balance is: ${newBalance:F2}");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Deposit failed for player {PlayerId} with amount {Amount}", 
                player.Id, amount);
            return CommandResult.Error($"Deposit failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during deposit for player {PlayerId} with amount {Amount}", 
                player.Id, amount);
            return CommandResult.Error("An unexpected error occurred during deposit.");
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
                throw new InvalidOperationException("Insufficient funds for withdrawal.");
            }

            // Perform withdrawal
            var oldBalance = player.Wallet.Balance;
            var newBalance = oldBalance - withdrawMoney;
            
            player.Wallet.Balance = newBalance;

            _logger.LogInformation("Withdrawal successful for player {PlayerId}. New balance: {NewBalance}", 
                player.Id, newBalance);

            return CommandResult.Success(
                $"Your withdrawal of ${amount:F2} was successful. Your current balance is: ${newBalance:F2}");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Withdrawal failed for player {PlayerId} with amount {Amount}", 
                player.Id, amount);
            return CommandResult.Error($"Withdrawal failed: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Withdrawal failed for player {PlayerId} with amount {Amount}", 
                player.Id, amount);
            return CommandResult.Error($"Withdrawal failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Withdrawal failed for player {PlayerId} with amount {Amount}", 
                player.Id, amount);
            return CommandResult.Error("An unexpected error occurred during withdrawal.");
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
                throw new InvalidOperationException("Insufficient funds for bet.");
            }

            // Perform bet
            var oldBalance = player.Wallet.Balance;
            var newBalance = oldBalance - betMoney;
            
            player.Wallet.Balance = newBalance;

            _logger.LogInformation("Bet placed: Player {PlayerId}, Amount: {Amount}, New Balance: {Balance}", 
                player.Id, betAmount, newBalance);

            return CommandResult.Success($"Bet of ${betAmount:F2} placed successfully.");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Bet placement failed for player {PlayerId} with bet amount {Amount}", 
                player.Id, betAmount);
            return CommandResult.Error($"Bet placement failed: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Bet placement failed for player {PlayerId} with bet amount {Amount}", 
                player.Id, betAmount);
            return CommandResult.Error($"Bet placement failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bet placement: Amount {Amount}", betAmount);
            return CommandResult.Error("Bet placement failed. Please try again.");
        }
    }

    public CommandResult AcceptWin(Player player, decimal winAmount)
    {
        try
        {
            if (winAmount <= 0)
            {
                return CommandResult.Error("Win amount must be positive.");
            }

            // Create money value object
            var winMoney = new Money(winAmount);

            // Perform win acceptance
            var oldBalance = player.Wallet.Balance;
            var newBalance = oldBalance + winMoney;
            
            player.Wallet.Balance = newBalance;

            _logger.LogInformation("Win accepted: Player {PlayerId}, Amount: {Amount}, New Balance: {Balance}", 
                player.Id, winAmount, newBalance);

            return CommandResult.Success($"Win of ${winAmount:F2} accepted successfully.");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Win acceptance failed for player {PlayerId} with amount {Amount}", 
                player.Id, winAmount);
            return CommandResult.Error($"Win acceptance failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during win acceptance: Amount {Amount}", winAmount);
            return CommandResult.Error("Win acceptance failed. Please try again.");
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