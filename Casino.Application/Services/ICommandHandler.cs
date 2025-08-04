using Casino.Core.Entities;
using Casino.Core.Results;

namespace Casino.Application.Services;

public interface ICommandHandler
{
    Task<CommandResult> HandleDepositAsync(Player player, decimal amount);
    Task<CommandResult> HandleWithdrawAsync(Player player, decimal amount);
    Task<CommandResult> HandleBetAsync(Player player, decimal betAmount);
    Task<CommandResult> HandleExitAsync();
}