using Casino.Core.Configurations;
using Casino.Core.Constants;
using Casino.Core.Entities;
using Casino.Core.Enums;
using Casino.Core.Results;
using Casino.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Casino.Infrastructure.Services;

public class SlotGameService : ISlotGameService
{
    private readonly IRandomNumberGeneratorService _rngService;
    private readonly ILogger<SlotGameService> _logger;
    private readonly IWalletService _walletService;
    private readonly IOptions<GameConfiguration> _gameConfiguration;

    public SlotGameService(
        IRandomNumberGeneratorService rngService,
        ILogger<SlotGameService> logger,
        IWalletService walletService,
        IOptions<GameConfiguration> gameConfiguration)
    {
        _rngService = rngService;
        _logger = logger;
        _walletService = walletService;
        _gameConfiguration = gameConfiguration;

    }

    /// <summary>
    /// Calculates the win amount based on the bet amount and the game result type
    /// </summary>
    /// <param name="betAmount">The bet amount</param>
    /// <param name="gameResultType">The game result type</param>
    /// <returns>The win amount</returns>
    public decimal CalculateWinAmount(decimal betAmount, GameResultType gameResultType, GameConfiguration gameConfiguration)
    {
        return gameResultType switch
        {
            GameResultType.Loss => 0m,
            GameResultType.SmallWin => betAmount * _rngService.GetRandomDecimal(1.0m, gameConfiguration.SmallWinMaxMultiplier),
            GameResultType.BigWin => betAmount * _rngService.GetRandomDecimal(gameConfiguration.BigWinMinMultiplier, gameConfiguration.BigWinMaxMultiplier),
            _ => throw new ArgumentException(UserMessages.InvalidGameResultType)
        };

    }

    /// <summary>
    /// Determines the game result based on the game configuration
    /// </summary>
    /// <param name="config">The game configuration</param>
    /// <returns>The game result</returns>
    public GameResultType DetermineGameResult(GameConfiguration config)
    {
        var randomValue = _rngService.GetRandomDecimal(0, 1);

        if (randomValue <= config.LossProbability)
        {
            return GameResultType.Loss;
        }
        else if (randomValue <= config.LossProbability + config.SmallWinProbability)
        {
            return GameResultType.SmallWin;
        }
        else
        {
            return GameResultType.BigWin;
        }
    }

    
    public CommandResult ProcessBet(Player player, decimal betAmount)
    {
        try
        {
            _logger.LogInformation(string.Format(CultureInfo.InvariantCulture, LogMessages.ProcessingBet,
                betAmount, player.Id));

            // Place the bet (deduct from wallet)
            var placeBetResult = _walletService.PlaceBet(player, betAmount);
            if (!placeBetResult.IsSuccess)
            {
                _logger.LogWarning(LogMessages.FailedToPlaceBet,
                    betAmount, player.Id, placeBetResult.Message);
                return placeBetResult;
            }

            // Determine game result
            var gameResult = DetermineGameResult(_gameConfiguration.Value);
            _logger.LogInformation(LogMessages.GameResultDetermined,
                gameResult, player.Id);

            // Act based on game result
            if (gameResult == GameResultType.SmallWin ||
                gameResult == GameResultType.BigWin)
            {
                var winAmount = CalculateWinAmount(betAmount, gameResult, _gameConfiguration.Value);
                _logger.LogInformation(LogMessages.WinAmountCalculated,
                    winAmount, player.Id);
                var acceptWinResult = _walletService.AcceptWin(player, winAmount);
                if (!acceptWinResult.IsSuccess)
                {
                    _logger.LogError(LogMessages.FailedToAcceptWin,
                        winAmount, player.Id, acceptWinResult.Message);
                    return acceptWinResult;
                }

                return CommandResult.Success(
                    string.Format(CultureInfo.InvariantCulture, UserMessages.AcceptWinSuccessful, winAmount, player.Wallet.Balance.Amount));
            }
            else
            {
                return CommandResult.Success(
                    string.Format(CultureInfo.InvariantCulture, UserMessages.NoLuck, player.Wallet.Balance.Amount));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                string.Format(CultureInfo.InvariantCulture, LogMessages.UnexpectedErrorProcessingBet,
                betAmount, player.Id));

            return CommandResult.Error(
                string.Format(CultureInfo.InvariantCulture, LogMessages.UnexpectedErrorProcessingBet,
                betAmount));
        }
    }
}