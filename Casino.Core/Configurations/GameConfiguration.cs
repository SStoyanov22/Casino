using System.Security.Cryptography;
using Casino.Core.Constants;
using Casino.Core.Enums;

namespace Casino.Core.Configurations;

/// <summary>
/// Configuration class for game rules and settings
/// </summary>
public class GameConfiguration
{
    // Bet limits
    public decimal MinimumBet { get; set; } = 1.0m;
    public decimal MaximumBet { get; set; } = 10.0m;

    // Game probabilities
    public decimal LossProbability { get; set; } = 0.5m;
    public decimal SmallWinProbability { get; set; } = 0.4m;
    public decimal BigWinProbability { get; set; } = 0.1m;

    // Multipliers
    public decimal SmallWinMaxMultiplier { get; set; } = 2.0m;
    public decimal BigWinMaxMultiplier { get; set; } = 10.0m;
    public decimal BigWinMinMultiplier { get; set; } = 2.0m;

    // Static instance
    public static GameConfiguration Instance { get; private set; } = new();

    public static void Initialize(GameConfiguration config)
    {
        Instance = config;
    }
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

    /// Validations
    public bool IsValidBetAmount(decimal amount)
    {   
        return amount >= MinimumBet && amount <= MaximumBet;
    }

    private bool ValidateProbabilities()
    {
        decimal totalProbability = LossProbability + SmallWinProbability + BigWinProbability;
        return Math.Abs(totalProbability - 1.0m) == 0m; 
    } 


} 