using System.Security.Cryptography;
using Casino.Core.Configurations;
using Casino.Core.Constants;
using Casino.Core.Enums;
using Casino.Infrastructure.Interfaces;

namespace Casino.Infrastructure.Services;

public class SlotGameService : ISlotGameService
{
    private readonly IRandomNumberGeneratorService _rngService;

    public SlotGameService(IRandomNumberGeneratorService rngService)
    {
        _rngService = rngService;
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

        if (randomValue < config.LossProbability)
        {
            return GameResultType.Loss;
        }
        else if (randomValue < config.LossProbability + config.SmallWinProbability)
        {
            return GameResultType.SmallWin;
        }
        else
        {
            return GameResultType.BigWin;
        }
    }

    /// <summary>
    /// Generates a random multiplier between a minimum and maximum value
    /// </summary>
    /// <param name="min">The minimum value</param>
    /// <param name="max">The maximum value</param>
    /// <returns>A random multiplier between the minimum and maximum value</returns>
    private static decimal GetRandomMultiplier(decimal min, decimal max)
    {
        // Generate random integer between 0 and 1000000 for precision
        int randomInt = RandomNumberGenerator.GetInt32(0, 1000001);
        
        // Convert to decimal between 0 and 1
        decimal randomDecimal = (decimal)randomInt / 1000000m;
        
        // Scale to range
        return min + (randomDecimal * (max - min));
    }
}