using Casino.Core.Configurations;
using Casino.Core.Constants;

namespace Casino.Core.ValueObjects;

public class BetAmount
{
    public decimal Amount { get; }

    public BetAmount(decimal amount)
    {
        if (amount < GameConfiguration.Instance.MinimumBet ||
         amount > GameConfiguration.Instance.MaximumBet)
        {
            throw new ArgumentException(
                string.Format(UserMessages.BetAmountOutMustBeBetween,
                GameConfiguration.Instance.MinimumBet,
                 GameConfiguration.Instance.MaximumBet),
                nameof(amount)
            );
        }
        Amount = amount;
    }

    public override string ToString() => $"{Amount}";

    public static implicit operator decimal(BetAmount bet) => bet.Amount;
}