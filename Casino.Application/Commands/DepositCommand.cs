using Casino.Core.Commands;
using Casino.Core.Results;
using Casino.Application.Services;
using Microsoft.Extensions.Logging;
using Casino.Core.DTOs;
using Casino.Core.Enums;
using Casino.Core.Constants;

namespace Casino.Application.Commands;

public class DepositCommand : ICommand<CommandResult>
{
    private readonly ILogger<DepositCommand> _logger;
    private readonly IWalletService _walletService;

    public CommandType CommandType => CommandType.Deposit;

    public DepositCommand(IWalletService walletService, ILogger<DepositCommand> logger)
    {
        _walletService = walletService;
        _logger = logger;
    }

   public Task<CommandResult> ExecuteAsync(CommandRequest request)
    {
        _logger.LogInformation(LogMessages.CommandExecutionStarted, 
                 typeof(DepositCommand).Name, request.Player.Id, request.Amount);

        var result =  _walletService.Deposit(request.Player, request.Amount);

        if (result.IsSuccess)
        {
            _logger.LogInformation(LogMessages.CommandExecutionCompleted, 
                 typeof(DepositCommand).Name, request.Player.Id, request.Player.Wallet.Balance);
        }
        else
        {
            _logger.LogWarning(LogMessages.CommandExecutionFailed,
            typeof(DepositCommand).Name, request.Player.Id, result.Message);
        }
        
        return Task.FromResult(result);
    }
}