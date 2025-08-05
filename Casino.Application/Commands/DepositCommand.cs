using Casino.Core.Commands;
using Casino.Core.Entities;
using Casino.Core.Results;
using Casino.Application.Services;
using Microsoft.Extensions.Logging;
using Casino.Core.DTOs;

namespace Casino.Application.Commands;

public class DepositCommand : BaseCommand<CommandResult>
{
    private readonly IWalletService _walletService;

    public DepositCommand(IWalletService walletService, ILogger<DepositCommand> logger)
    : base(logger)
    {
        _walletService = walletService;
    }

   public override Task<CommandResult> ExecuteAsync(CommandRequest request)
    {
        _logger.LogInformation("Deposit Command executing for player {PlayerId} with amount {Amount}", 
                request.Player.Id, request.Amount);

        var result =  _walletService.Deposit(request.Player, request.Amount);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Deposit Command completed successfully for player {PlayerId}. New balance: {NewBalance}", 
                request.Player.Id, request.Player.Wallet.Balance);
        }
        else
        {
            _logger.LogWarning("Deposit Command failed for player {PlayerId}: {Error}", 
                request.Player.Id, result.Message);
        }
        
        return Task.FromResult(result);
    }
}