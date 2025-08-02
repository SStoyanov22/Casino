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
    /// Probability of losing (0.0 to 1.0)
    /// </summary>
    public decimal LossProbability { get; set; } = 0.5m;

    /// <summary>
    /// Probability of winning up to x2 the bet amount (0.0 to 1.0)
    /// </summary>
    public decimal SmallWinProbability { get; set; } = 0.4m;

    /// <summary>
    /// Probability of winning between x2 and x10 the bet amount (0.0 to 1.0)
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
    /// Validates that the configuration is valid
    /// </summary>
    /// <returns>True if configuration is valid, false otherwise</returns>
    public bool IsValid()
    {
        return MinimumBet > 0 &&
               MaximumBet >= MinimumBet &&
               LossProbability >= 0 && LossProbability <= 1 &&
               SmallWinProbability >= 0 && SmallWinProbability <= 1 &&
               BigWinProbability >= 0 && BigWinProbability <= 1 &&
               SmallWinMaxMultiplier > 0 &&
               BigWinMaxMultiplier > BigWinMinMultiplier &&
               BigWinMinMultiplier > 0;
    }
} 