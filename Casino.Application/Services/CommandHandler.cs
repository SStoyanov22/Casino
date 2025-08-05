using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Casino.Core.Results;
using Casino.Core.Entities;
using Casino.Core.Configurations;
using Casino.Application.Commands;
using Casino.Core.Enums;
using Casino.Core.Commands;
using Microsoft.Extensions.DependencyInjection;
using Casino.Core.DTOs;

namespace Casino.Application.Services;

public class CommandHandler : ICommandHandler
{
    private readonly IServiceProvider _serviceProvider;
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

    public async Task<CommandResult> ExecuteCommandAsync(string command, decimal amount, Player player, ICommandHandler commandHandler)
    {
        var commandType = GetCommandType(command);
        return commandType switch
        {
            CommandType.Deposit => await ExecuteCommand<DepositCommand>(amount, player),
            CommandType.Withdraw => await ExecuteCommand<WithdrawCommand>(amount, player),
            CommandType.Bet => await ExecuteCommand<BetCommand>(amount, player),
            CommandType.Exit => await ExecuteCommand<ExitCommand>(amount, player),
            _ =>  CommandResult.Error("Invalid command")
        };
    }

    private async Task<CommandResult> ExecuteCommand<T>(decimal amount, Player player) where T : ICommand<CommandResult>
    {
        var command = _serviceProvider.GetRequiredService<T>();
        var commandRequest = new CommandRequest(amount, player);
        
        return await command.ExecuteAsync(commandRequest);
    }
    private CommandType GetCommandType(string command)
    {
        return command.ToLowerInvariant() switch
        {
            "deposit" => CommandType.Deposit,
            "withdraw" => CommandType.Withdraw,
            "bet" => CommandType.Bet,
            "exit" => CommandType.Exit,
            _ => CommandType.Exit // Default to exit for invalid commands
        };
    }
}