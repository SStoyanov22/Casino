using Casino.Core.Configurations;
using Casino.Core.Constants;

namespace Casino.Core.ValueObjects;

public class BetAmount
{
    public decimal Amount { get; }

    public BetAmount(decimal amount)
    {
        if (amount < GameConfiguration.Instance.MinimumBet || amount > GameConfiguration.Instance.MaximumBet)
        {
            throw new ArgumentException($"Bet must be between ${GameConfiguration.Instance.MinimumBet:F2} and ${GameConfiguration.Instance.MaximumBet:F2}");
        }
        Amount = amount;
    }

    public static implicit operator decimal(BetAmount bet) => bet.Amount;
}