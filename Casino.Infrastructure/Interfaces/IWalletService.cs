using Casino.Core.Entities;
using Casino.Core.Results;

namespace Casino.Infrastructure.Interfaces;

public interface IWalletService
{
    CommandResult Deposit(Player player, decimal amount);
    CommandResult Withdraw(Player player, decimal amount);
    CommandResult AcceptLoss(Player player, decimal betAmount);
    CommandResult AcceptWin(Player player, decimal winAmount);
    decimal GetBalance(Player player);
    bool HasSufficientFunds(Player player, decimal amount);
}