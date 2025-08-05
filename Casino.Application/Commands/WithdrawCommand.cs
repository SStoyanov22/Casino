using Casino.Core.Commands;
using Casino.Core.Entities;
using Casino.Core.Results;
using Casino.Application.Services;
using Microsoft.Extensions.Logging;
using Casino.Core.DTOs;

namespace Casino.Application.Commands;

public class WithdrawCommand : BaseCommand<CommandResult>
{
    private readonly IWalletService _walletService;

    public WithdrawCommand(IWalletService walletService, ILogger<WithdrawCommand> logger)
    : base(logger)
    {
        _walletService = walletService;
    }

    public override Task<CommandResult> ExecuteAsync(CommandRequest request)
    {
        _logger.LogInformation("Withdraw Command executing for player {PlayerId} with amount {Amount}", 
            request.Player.Id, request.Amount);

        var result = _walletService.Withdraw(request.Player, request.Amount);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("Withdrawal Command completed successfully for player {PlayerId}. New balance: {NewBalance}", 
                request.Player.Id, request.Player.Wallet.Balance);
        }
        else
        {
            _logger.LogWarning("Withdrawal Command failed for player {PlayerId}: {Error}", 
                request.Player.Id, result.Message);
        }
        
        return Task.FromResult(result);
    }
}