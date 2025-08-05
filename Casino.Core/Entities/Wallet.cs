using Casino.Core.ValueObjects;

namespace Casino.Core.Entities;

public class Wallet
{
    public Money Balance { get; set; }  

    public Wallet()
    {
        Balance = new Money(0);
    }
}