using Casino.Core.Commands;
using Casino.Core.Entities;
using Casino.Core.Configurations;
using Casino.Core.Enums;
using Casino.Application.Services;
using Microsoft.Extensions.Logging;
using Casino.Core.ValueObjects;
using Casino.Core.Results;
using Casino.Core.DTOs;

namespace Casino.Application.Commands;

public class BetCommand : BaseCommand<CommandResult>
{
    
    private readonly WalletService _walletService;

    public BetCommand(ILogger logger, 
    WalletService walletService) 
        : base(logger)
    {
        _walletService = walletService;
    }

    public override Task<CommandResult> ExecuteAsync(CommandRequest request)
    {
        _logger.LogInformation("Bet Command executing for player {PlayerId} with amount {Amount}", 
            request.Player.Id, request.Amount);

        var result = _walletService.PlaceBet(request.Player, request.Amount);

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