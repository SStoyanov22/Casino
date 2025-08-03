using Casino.Core.Commands;
using Casino.Core.Entities;
using Casino.Core.Configurations;
using Microsoft.Extensions.Options;
using Casino.Application.Commands;

namespace Casino.Application.Services;

public class CommandHandler : ICommandHandler
{
    private readonly ILogger<CommandHandler> _logger;
    private readonly IGameEngine _gameEngine;
    private readonly GameConfiguration _gameConfig;

    public CommandHandler(ILogger<CommandHandler> logger, IGameEngine gameEngine, 
                        IOptions<GameConfiguration> gameConfig)
    {
        _logger = logger;
        _gameEngine = gameEngine;
        _gameConfig = gameConfig.Value;
    }

    public async Task<CommandResult> ExecuteDepositAsync(Player player, decimal amount)
    {
        _logger.LogInformation("CommandHandler: Executing deposit for player {PlayerId} with amount {Amount}", 
            player.Id, amount);

        var command = new DepositCommand(_logger, player, amount);
        return await command.ExecuteAsync();
    }

    public async Task<CommandResult> ExecuteWithdrawAsync(Player player, decimal amount)
    {
        _logger.LogInformation("CommandHandler: Executing withdrawal for player {PlayerId} with amount {Amount}", 
            player.Id, amount);

        var command = new WithdrawCommand(_logger, player, amount);
        return await command.ExecuteAsync();
    }

    public async Task<CommandResult> ExecuteBetAsync(Player player, decimal betAmount)
    {
        _logger.LogInformation("CommandHandler: Executing bet for player {PlayerId} with amount {Amount}", 
            player.Id, betAmount);

        var command = new BetCommand(_logger, player, betAmount, _gameEngine, _gameConfig);
        return await command.ExecuteAsync();
    }

    public async Task<CommandResult> ExecuteExitAsync()
    {
        _logger.LogInformation("CommandHandler: Executing exit command");

        var command = new ExitCommand(_logger);
        return await command.ExecuteAsync();
    }
}