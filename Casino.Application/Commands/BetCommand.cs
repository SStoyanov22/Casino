using Casino.Core.Commands;
using Casino.Core.Entities;
using Casino.Core.Configurations;
using Casino.Core.Enums;
using Casino.Application.Services;
using Microsoft.Extensions.Logging;
using Casino.Core.ValueObjects;
using Casino.Core.Results;

namespace Casino.Application.Commands;

public class BetCommand : BaseCommand<CommandResult>
{
    private readonly Player _player;
    private readonly decimal _betAmount;
    private readonly IGameEngine _gameEngine;
    private readonly GameConfiguration _gameConfig;

    public BetCommand(ILogger logger, Player player, decimal betAmount, 
                     IGameEngine gameEngine, GameConfiguration gameConfig) 
        : base(logger)
    {
        _player = player;
        _betAmount = betAmount;
        _gameEngine = gameEngine;
        _gameConfig = gameConfig;
    }

    public override Task<CommandResult> ExecuteAsync()
    {
        try
        {
            Logger.LogInformation("Executing BetCommand for player {PlayerId} with amount {Amount}", 
                _player.Id, _betAmount);

            var betAmount = new BetAmount(_betAmount);

            // Validate bet amount
            if (!_gameConfig.IsValidBetAmount(betAmount))
            {
                return Task.FromResult(CommandResult.Error($"Bet amount must be between ${_gameConfig.MinimumBet} and ${_gameConfig.MaximumBet}"));
            }

            // Place bet
            _player.Wallet.PlaceBet(betAmount);

            // Determine game result using game engine
            var resultType = _gameEngine.DetermineGameResult(_gameConfig);
            var winAmount = _gameEngine.CalculateWinAmount(betAmount, resultType, _gameConfig);

            // Accept winnings if any
            if (winAmount > 0)
            {
                var winnings = new Money(winAmount);
                _player.Wallet.AcceptWin(winnings);
            }

            var resultMessage = resultType switch
            {
                GameResultType.Loss => $"No luck this time! Your current balance is: ${_player.Wallet.Balance:F2}",
                GameResultType.SmallWin => $"Congrats - you won ${winAmount:F2}! Your current balance is: ${_player.Wallet.Balance:F2}",
                GameResultType.BigWin => $"Congrats - you won ${winAmount:F2}! Your current balance is: ${_player.Wallet.Balance:F2}",
                _ => "Unknown result"
            };

            Logger.LogInformation("Bet completed for player {PlayerId}. Result: {ResultType}, Win Amount: {WinAmount}, New Balance: {NewBalance}", 
                _player.Id, resultType, winAmount, _player.Wallet.Balance);

            return Task.FromResult(CommandResult.Success(resultMessage, _player.Wallet.Balance));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An unexpected error occurred during bet for player {PlayerId} with amount {Amount}", 
                _player.Id, _betAmount);
            return Task.FromResult(CommandResult.Error("An unexpected error occurred during betting."));
        }
    }
}