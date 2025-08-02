using System.Security.Cryptography;
using Casino.Core.Constants;
using Casino.Core.Enums;

namespace Casino.Core.Configurations;

/// <summary>
/// Configuration class for game rules and settings
/// </summary>
public class GameConfiguration
{
    /// <summary>
    /// Minimum bet amount allowed
    /// </summary>
    public decimal MinimumBet { get; set; } = 1.0m;

    /// <summary>
    /// Maximum bet amount allowed
    /// </summary>
    public decimal MaximumBet { get; set; } = 10.0m;

    /// <summary>
    /// Probability of losing
    /// </summary>
    public decimal LossProbability { get; set; } = 0.5m;

    /// <summary>
    /// Probability of winning up to x2 the bet amount
    /// </summary>
    public decimal SmallWinProbability { get; set; } = 0.4m;

    /// <summary>
    /// Probability of winning between x2 and x10 the bet amount
    /// </summary>
    public decimal BigWinProbability { get; set; } = 0.1m;

    /// <summary>
    /// Maximum multiplier for small wins (up to x2)
    /// </summary>
    public decimal SmallWinMaxMultiplier { get; set; } = 2.0m;

    /// <summary>
    /// Maximum multiplier for big wins (x2 to x10)
    /// </summary>
    public decimal BigWinMaxMultiplier { get; set; } = 10.0m;

    /// <summary>
    /// Minimum multiplier for big wins (x2 to x10)
    /// </summary>
    public decimal BigWinMinMultiplier { get; set; } = 2.0m;

    /// <summary>
    /// Validates that the game configuration is valid by checking bet ranges, probability values, 
    /// multiplier settings, and ensuring probabilities sum to 1.0
    /// </summary>
    /// <returns>True if all configuration values are valid, false otherwise</returns>
    public bool IsValid()
    {
        return MinimumBet > 0 &&
               MaximumBet >= MinimumBet &&
               LossProbability >= 0 && LossProbability <= 1 &&
               SmallWinProbability >= 0 && SmallWinProbability <= 1 &&
               BigWinProbability >= 0 && BigWinProbability <= 1 &&
               SmallWinMaxMultiplier > 0 &&
               BigWinMaxMultiplier > BigWinMinMultiplier &&
               BigWinMinMultiplier > 0 &&
               ValidateProbabilities();
    }

    /// <summary>
    /// Validates that the bet amount is within the allowed range
    /// </summary>
    /// <param name="amount">The bet amount to validate</param>
    /// <returns>True if the bet amount is valid, false otherwise</returns>
    public bool IsValidBetAmount(decimal amount)
    {   
        return amount >= MinimumBet && amount <= MaximumBet;
    }

    /// <summary>
    /// Calculates the win amount based on the bet amount and the game result type
    /// </summary>
    /// <param name="betAmount">The bet amount</param>
    /// <param name="resultType">The game result type</param>
    /// <returns>The win amount</returns>
    public decimal CalculateWinAmount(decimal betAmount, GameResultType resultType)
    {
        return resultType switch
        {
            GameResultType.Loss => 0m,
            GameResultType.SmallWin => betAmount *  GetRandomMultiplier(1.0m, SmallWinMaxMultiplier),
            GameResultType.BigWin => betAmount * GetRandomMultiplier(BigWinMinMultiplier, BigWinMaxMultiplier),
            _ => throw new ArgumentException(ExceptionMessages.InvalidGameResultType)
        };

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
        
        // Scale to your range
        return min + (randomDecimal * (max - min));
    }

    /// <summary>
    /// Validates that all probabilities sum to 1.0
    /// </summary>
    /// <returns>True if probabilities sum to 1.0, false otherwise</returns>
    private bool ValidateProbabilities()
    {
        decimal totalProbability = LossProbability + SmallWinProbability + BigWinProbability;
        return Math.Abs(totalProbability - 1.0m) = 0m; 
    } 


} 