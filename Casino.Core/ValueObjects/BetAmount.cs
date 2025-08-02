using Casino.Core.Constants;

namespace Casino.Core.ValueObjects;

public class BetAmount
{
    public decimal Amount {get;}
    public BetAmount(decimal amount)
    {
        if (amount < 1 || amount > 10) 
        {
            throw new ArgumentException(ExceptionMessages.BetAmountMustBeBetweenOneAndTen);
        }

        Amount = amount;
    }

    public static implicit operator decimal(BetAmount bet) => bet.Amount;
}