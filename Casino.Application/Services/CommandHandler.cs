using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Casino.Core.Results;
using Casino.Core.Entities;
using Casino.Core.Configurations;
using Casino.Application.Commands;

namespace Casino.Application.Services;

public class CommandHandler : ICommandHandler
{
    private readonly ILogger<CommandHandler> _logger;
    private readonly ISlotGameService _gameEngine;
    private readonly GameConfiguration _gameConfig;

    public CommandHandler(ILogger<CommandHandler> logger, ISlotGameService gameEngine, 
                        IOptions<GameConfiguration> gameConfig)
    {
        _logger = logger;
        _gameEngine = gameEngine;
        _gameConfig = gameConfig.Value;
    }

    public async Task<CommandResult> HandleDepositAsync(Player player, decimal amount)
    {
        _logger.LogInformation("CommandHandler: Executing deposit for player {PlayerId} with amount {Amount}", 
            player.Id, amount);

        var command = new DepositCommand(_logger, player, amount);
        return await command.ExecuteAsync();
    }

    public async Task<CommandResult> HandleWithdrawAsync(Player player, decimal amount)
    {
        _logger.LogInformation("CommandHandler: Executing withdrawal for player {PlayerId} with amount {Amount}", 
            player.Id, amount);

        var command = new WithdrawCommand(_logger, player, amount);
        return await command.ExecuteAsync();
    }

    public async Task<CommandResult> HandleBetAsync(Player player, decimal betAmount)
    {
        _logger.LogInformation("CommandHandler: Executing bet for player {PlayerId} with amount {Amount}", 
            player.Id, betAmount);

        var command = new BetCommand(_logger, player, betAmount, _gameEngine, _gameConfig);
        return await command.ExecuteAsync();
    }

    public async Task<CommandResult> HandleExitAsync()
    {
        _logger.LogInformation("CommandHandler: Executing exit command");

        var command = new ExitCommand(_logger);
        return await command.ExecuteAsync();
    }
}