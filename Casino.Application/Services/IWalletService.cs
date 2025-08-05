using Casino.Core.Entities;
using Casino.Core.ValueObjects;
using Casino.Core.Results;

namespace Casino.Application.Services;

public interface IWalletService
{
    CommandResult Deposit(Player player, decimal amount);
    CommandResult Withdraw(Player player, decimal amount);
    CommandResult PlaceBet(Player player, decimal betAmount);
    CommandResult AcceptWin(Player player, decimal winAmount);
    decimal GetBalance(Player player);
    bool HasSufficientFunds(Player player, decimal amount);
}