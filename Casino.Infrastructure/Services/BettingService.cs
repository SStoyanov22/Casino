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

public class BettingService : IBettingService
{
    private readonly ILogger<BettingService> _logger;
    private readonly IWalletService _walletService;
    private readonly ISlotGameService _slotGameService;
    private readonly IOptions<GameConfiguration> _gameConfiguration;

    public BettingService(
        ILogger<BettingService> logger,
        IWalletService walletService,
        ISlotGameService slotGameService,
        IOptions<GameConfiguration> gameConfiguration)
    {
        _logger = logger;
        _walletService = walletService;
        _slotGameService = slotGameService;
        _gameConfiguration = gameConfiguration;
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
            var gameResult = _slotGameService.DetermineGameResult(_gameConfiguration.Value);
            _logger.LogInformation(LogMessages.GameResultDetermined,
                gameResult, player.Id);

            // Act based on game result
            if (gameResult == GameResultType.SmallWin ||
                gameResult == GameResultType.BigWin)
            {
                var winAmount = _slotGameService.CalculateWinAmount(betAmount, gameResult, _gameConfiguration.Value);
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