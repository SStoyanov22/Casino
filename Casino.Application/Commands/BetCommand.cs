using Casino.Core.Commands;
using Casino.Core.Configurations;
using Casino.Core.Enums;
using Casino.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Casino.Core.Results;
using Casino.Core.DTOs;
using Casino.Core.Constants;
using Microsoft.Extensions.Options;

namespace Casino.Application.Commands;

public class BetCommand : ICommand<CommandResult>
{
    private readonly ILogger<BetCommand> _logger;
    private readonly ISlotGameService _slotGameService;
    private readonly IValidationService _validationService;
    public CommandType CommandType => CommandType.Bet;
    

    public BetCommand(ILogger<BetCommand> logger, 
    IWalletService walletService,
    ISlotGameService slotGameService,
    IValidationService validationService,
    IOptions<GameConfiguration> gameConfiguration) 
    {
        _logger = logger;
        _slotGameService = slotGameService;
        _validationService = validationService;
    }

    public Task<CommandResult> ExecuteAsync(CommandRequest request)
    {
        _logger.LogInformation(LogMessages.CommandExecutionStarted,
         typeof(BetCommand).Name, request.Player.Id, request.Amount);

        // Validate bet amount first
        var validationResult = _validationService.ValidateAmount(CommandType.Bet, request.Amount);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(LogMessages.CommandExecutionFailed, 
                typeof(BetCommand).Name, request.Player.Id, validationResult.ErrorMessage);
            return Task.FromResult(CommandResult.Error(validationResult.ErrorMessage));
        }

        // Process the bet
        var result = _slotGameService.ProcessBet(request.Player, request.Amount);

        if (result.IsSuccess)
        {
            _logger.LogInformation(LogMessages.CommandExecutionCompleted, 
                 typeof(BetCommand).Name, request.Player.Id, request.Player.Wallet.Balance);
        }
        else
        {
            _logger.LogWarning(LogMessages.CommandExecutionFailed, 
            typeof(BetCommand).Name, request.Player.Id, result.Message);
        }

        return Task.FromResult(result);
    }
}