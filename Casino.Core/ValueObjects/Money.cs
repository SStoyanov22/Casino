using Casino.Core.Constants;

namespace Casino.Core.ValueObjects;

public class Money 
{
    public decimal Amount { get; }
    public Money (decimal amount)
    {
        if (amount < 0) 
        {
            throw new ArgumentException(ExceptionMessages.MoneyAmountCannotBeNegative);
        }
        Amount = amount;
    }

    public static implicit operator decimal(Money money) => money.Amount;
    public static implicit operator Money(decimal amount) => new(amount);
}