using Casino.Core.Commands;
using Casino.Core.Entities;
using Casino.Core.Results;
using Casino.Core.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Casino.Application.Commands;

public class WithdrawCommand : BaseCommand<CommandResult>
{
    private readonly Player _player;
    private readonly decimal _amount;

    public WithdrawCommand(ILogger logger, Player player, decimal amount) 
        : base(logger)
    {
        _player = player;
        _amount = amount;
    }

    public override Task<CommandResult> ExecuteAsync()
    {
        try
        {
            Logger.LogInformation("Executing WithdrawCommand for player {PlayerId} with amount {Amount}", 
                _player.Id, _amount);

            var money = new Money(_amount);
            
            _player.Wallet.Withdraw(money);

            Logger.LogInformation("Withdrawal successful for player {PlayerId}. New balance: {NewBalance}", 
                _player.Id, _player.Wallet.Balance);

            return Task.FromResult(CommandResult.Success(
                $"Your withdrawal of ${_amount:F2} was successful. Your current balance is: ${_player.Wallet.Balance:F2}",
                _player.Wallet.Balance));
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning(ex, "Withdrawal failed for player {PlayerId} with amount {Amount}", 
                _player.Id, _amount);
            return Task.FromResult(CommandResult.Error($"Withdrawal failed: {ex.Message}"));
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogWarning(ex, "Withdrawal failed for player {PlayerId} with amount {Amount}", 
                _player.Id, _amount);
            return Task.FromResult(CommandResult.Error($"Withdrawal failed: {ex.Message}"));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An unexpected error occurred during withdrawal for player {PlayerId} with amount {Amount}", 
                _player.Id, _amount);
            return Task.FromResult(CommandResult.Error("An unexpected error occurred during withdrawal."));
        }
    }
}