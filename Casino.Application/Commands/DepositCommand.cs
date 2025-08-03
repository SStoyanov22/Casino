using Casino.Core.Results;
using Casino.Core.Entities;
using Casino.Core.ValueObjects;
using Microsoft.Extensions.Logging;
using Casino.Core.Commands;

namespace Casino.Application.Commands;

public class DepositCommand : BaseCommand<CommandResult>
{
    private readonly Player _player;
    private readonly decimal _amount;

    public DepositCommand(ILogger logger, Player player, decimal amount) 
        : base(logger)
    {
        _player = player;
        _amount = amount;
    }

    public override Task<CommandResult> ExecuteAsync()
    {
        try
        {
            var money = new Money(_amount);
            Logger.LogInformation("Executing DepositCommand for player {PlayerId} with amount {Amount}", 
                _player.Id, _amount);

            _player.Wallet.Deposit(money);
            
            Logger.LogInformation("Deposit successful for player {PlayerId}. New balance: {NewBalance}", 
                _player.Id, _player.Wallet.Balance);
            
            return Task.FromResult(CommandResult.Success(
                $"Your deposit of ${_amount:F2} was successful. Your current balance is: ${_player.Wallet.Balance:F2}",
                _player.Wallet.Balance));
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning(ex, "Deposit failed for player {PlayerId} with amount {Amount}", 
                _player.Id, _amount);
            return Task.FromResult(CommandResult.Error($"Deposit failed: {ex.Message}"));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An unexpected error occurred during deposit for player {PlayerId} with amount {Amount}", 
                _player.Id, _amount);
            return Task.FromResult(CommandResult.Error("An unexpected error occurred during deposit."));
        }
    }
}