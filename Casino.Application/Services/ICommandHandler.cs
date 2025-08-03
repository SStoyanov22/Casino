using Casino.Core.Entities;
using Casino.Core.Results;

namespace Casino.Application.Services;

public interface ICommandHandler
{
    Task<CommandResult> ExecuteDepositAsync(Player player, decimal amount);
    Task<CommandResult> ExecuteWithdrawAsync(Player player, decimal amount);
    Task<CommandResult> ExecuteBetAsync(Player player, decimal betAmount);
    Task<CommandResult> ExecuteExitAsync();
}