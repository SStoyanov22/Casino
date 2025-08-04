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
    private readonly ISlotGameService _gameService;
    private readonly GameConfiguration _gameConfig;

    public CommandHandler(ILogger<CommandHandler> logger, ISlotGameService gameService, 
                        IOptions<GameConfiguration> gameConfig)
    {
        _logger = logger;
        _gameService = gameService;
        _gameConfig = gameConfig.Value;
    }

    public async Task<CommandResult> HandleDepositAsync(Player player, decimal amount)
    {
        var command = new DepositCommand(_logger, player, amount);
        return await command.ExecuteAsync();
    }

    public async Task<CommandResult> HandleWithdrawAsync(Player player, decimal amount)
    {
        var command = new WithdrawCommand(_logger, player, amount);
        return await command.ExecuteAsync();
    }

    public async Task<CommandResult> HandleBetAsync(Player player, decimal betAmount)
    {
        var command = new BetCommand(_logger, player, betAmount, _gameService, _gameConfig);
        return await command.ExecuteAsync();
    }

    public async Task<CommandResult> HandleExitAsync()
    {
        var command = new ExitCommand(_logger);
        return await command.ExecuteAsync();
    }
}