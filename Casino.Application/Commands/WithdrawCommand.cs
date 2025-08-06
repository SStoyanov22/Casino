using Casino.Core.Commands;
using Casino.Core.Results;
using Casino.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Casino.Core.DTOs;
using Casino.Core.Enums;
using Casino.Core.Constants;

namespace Casino.Application.Commands;

public class WithdrawCommand : ICommand<CommandResult>
{
    private readonly ILogger<WithdrawCommand> _logger;
    private readonly IWalletService _walletService;
    private readonly IValidationService _validationService;
    public CommandType CommandType => CommandType.Withdraw;

    public WithdrawCommand(IWalletService walletService, ILogger<WithdrawCommand> logger, IValidationService validationService)
    {
        _walletService = walletService;
        _logger = logger;
        _validationService = validationService;
    }

    public Task<CommandResult> ExecuteAsync(CommandRequest request)
    {
        _logger.LogInformation(LogMessages.CommandExecutionStarted, 
            typeof(WithdrawCommand).Name, request.Player.Id, request.Amount);

        var validationResult = _validationService.ValidateAmount(CommandType, request.Amount, request.Player.Wallet.Balance);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(LogMessages.CommandExecutionFailed, 
                typeof(WithdrawCommand).Name, request.Player.Id, validationResult.ErrorMessage);
            return Task.FromResult(CommandResult.Error(validationResult.ErrorMessage));
        }

        var result = _walletService.Withdraw(request.Player, request.Amount);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation(LogMessages.CommandExecutionCompleted, 
                typeof(WithdrawCommand).Name, request.Player.Id, request.Player.Wallet.Balance);
        }
        else
        {
            _logger.LogWarning(LogMessages.CommandExecutionFailed, 
            typeof(WithdrawCommand).Name, request.Player.Id, result.Message);
        }
        
        return Task.FromResult(result);
    }
}